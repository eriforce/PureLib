using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using PureLib.Common;
using System.ComponentModel;

namespace PureLib.Common.Entities {
    public sealed class ResumableWebClient : IAsyncWebClient, IDisposable {
        private ResumableInternalWebClient client;

        public event AsyncCompletedEventHandler DownloadFileCompleted;
        public event EventHandler RequestRangeNotSatisfiable;

        public ResumableWebClient()
            : this(null, null) {
        }

        public ResumableWebClient(string referer)
            : this(referer, null) {
        }

        public ResumableWebClient(CookieContainer cookies)
            : this(null, cookies) {
        }

        public ResumableWebClient(string referer, CookieContainer cookies) {
            client = new ResumableInternalWebClient(referer, cookies);
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(OpenReadCompleted);
        }

        public void DownloadFileAsync(Uri address, string fileName) {
            if (fileName.IsNullOrEmpty())
                throw new ArgumentNullException("FileName cannot be empty.");

            client.FileName = fileName;
            client.OpenReadAsync(address);
        }

        public void CancelAsync() {
            client.CancelAsync();
        }

        public void Dispose() {
            client.Dispose();
        }

        private void OpenReadCompleted(object sender, OpenReadCompletedEventArgs e) {
            if (!e.Cancelled) {
                string fileName = ((ResumableInternalWebClient)sender).FileName;
                using (FileStream stream = new FileStream(fileName, FileMode.Append)) {
                    try {
                        e.Result.CopyTo(stream);
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

        public ResumableInternalWebClient(string referer, CookieContainer cookies)
            : base(referer, cookies) {
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
