using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using PureLib.Common;

namespace PureLib.Web {
    public sealed class ResumableWebClient : IAsyncWebClient, IDisposable {
        private ResumableInternalWebClient _client;

        public event AsyncCompletedEventHandler DownloadFileCompleted;
        public event EventHandler RequestRangeNotSatisfiable;

        public ResumableWebClient()
            : this(null, null, null, null) {
        }

        public ResumableWebClient(string referer)
            : this(referer, null, null, null) {
        }

        public ResumableWebClient(string userName, string password)
            : this(null, userName, password, null) {
        }

        public ResumableWebClient(CookieContainer cookies)
            : this(null, null, null, cookies) {
        }

        public ResumableWebClient(string referer, string userName, string password, CookieContainer cookies) {
            _client = new ResumableInternalWebClient(referer, userName, password, cookies);
            _client.OpenReadCompleted += new OpenReadCompletedEventHandler(OpenReadCompleted);
        }

        public void DownloadFileAsync(Uri address, string fileName) {
            if (fileName.IsNullOrEmpty())
                throw new ArgumentNullException("FileName cannot be empty.");

            _client.FileName = fileName;
            _client.OpenReadAsync(address);
        }

        public void CancelAsync() {
            _client.CancelAsync();
        }

        public void Dispose() {
            _client.Dispose();
        }

        private void OpenReadCompleted(object sender, OpenReadCompletedEventArgs e) {
            if (!e.Cancelled) {
                string fileName = ((ResumableInternalWebClient)sender).FileName;
                try {
                    using (FileStream stream = new FileStream(fileName, FileMode.Append)) {
                        e.Result.CopyTo(stream);
                    }
                    OnDownloadFileCompleted(false);
                }
                catch (WebException) {
                    OnDownloadFileCompleted(true);
                }
                catch (TargetInvocationException) {
                    OnRequestRangeNotSatisfiable();
                }
            }
        }

        private void OnDownloadFileCompleted(bool isCancelled) {
            if (DownloadFileCompleted != null)
                DownloadFileCompleted(this, new AsyncCompletedEventArgs(null, isCancelled, null));
        }

        private void OnRequestRangeNotSatisfiable() {
            if (RequestRangeNotSatisfiable != null)
                RequestRangeNotSatisfiable(this, new EventArgs());
        }
    }

    internal class ResumableInternalWebClient : AdvancedWebClient {
        public string FileName { get; set; }

        public ResumableInternalWebClient(string referer, string userName, string password, CookieContainer cookies)
            : base(referer, userName, password, cookies) {
        }

        protected override WebRequest GetWebRequest(Uri address) {
            if (FileName.IsNullOrEmpty())
                throw new ArgumentNullException("FileName cannot be empty.");

            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            FileInfo file = new FileInfo(FileName);
            if (file.Exists)
                request.AddRange(file.Length);
            return request;
        }
    }
}
