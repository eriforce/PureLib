using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PureLib.Common;

namespace PureLib.Web {
    public class AdvancedWebClient : WebClient {
        private ConcurrentDictionary<Uri, DownloadContext> _uriContexts =
            new ConcurrentDictionary<Uri, DownloadContext>();
        private FileExistsHandling _fileExistsHandling;

        public event EventHandler<EventArgs<HttpWebRequest>> SetRequest;
        public event EventHandler<EventArgs<HttpWebResponse>> GotResponse;
        public event EventHandler<EventArgs<DownloadPathChange>> Redirected;
        public event EventHandler<EventArgs<DownloadPathChange>> FileExists;

        public AdvancedWebClient(FileExistsHandling fileExistsHandling = FileExistsHandling.Rename) {
            _fileExistsHandling = fileExistsHandling;
        }

        public Task DownloadAsync(string address, string fullPath) {
            return DownloadAsync(new Uri(address), fullPath, CancellationToken.None);
        }

        public async Task DownloadAsync(Uri address, string fullPath, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            DownloadContext downloadContext = new DownloadContext();
            if (!_uriContexts.TryAdd(address, downloadContext))
                throw new ApplicationException("The url is downloading.");

            HttpWebResponse response = (HttpWebResponse)(await HeadAsync(address).ConfigureAwait(false));
            if (address != response.ResponseUri && Redirected != null) {
                DownloadPathChange redirectContext = new DownloadPathChange(response.ResponseUri) {
                    FullPath = fullPath,
                };
                Redirected(this, new EventArgs<DownloadPathChange>(redirectContext));
                fullPath = redirectContext.FullPath;
            }

            FileInfo file = new FileInfo(fullPath);
            downloadContext.CurrentFileLength = !file.Exists ? 0 : file.Length;

            try {
                using (cancellationToken.Register(CancelAsync))
                using (Stream responseStream = await OpenReadTaskAsync(address).ConfigureAwait(false))
                using (FileStream fileStream = new FileStream(fullPath, FileMode.OpenOrCreate)) {
                    if (downloadContext.CurrentFileLength > 0 && downloadContext.Response.IsRangeSupported())
                        fileStream.Seek(downloadContext.CurrentFileLength, SeekOrigin.Begin);

                    await responseStream.CopyToAsync(fileStream).ConfigureAwait(false);
                    await fileStream.FlushAsync().ConfigureAwait(false);
                }
            }
            finally {
                _uriContexts.TryRemove(address, out DownloadContext value);
            }
        }

        protected override WebRequest GetWebRequest(Uri address) {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            SetRequest?.Invoke(this, new EventArgs<HttpWebRequest>(request));

            long length = _uriContexts[address].CurrentFileLength;
            if (length > 0)
                request.AddRange(length);

            _uriContexts[address].Request = request;

            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result) {
            HttpWebResponse response = (HttpWebResponse)base.GetWebResponse(request, result);
            GotResponse?.Invoke(this, new EventArgs<HttpWebResponse>(response));

            _uriContexts[request.RequestUri].Response = response;

            return response;
        }

        private Task<WebResponse> HeadAsync(Uri address) {
            HttpWebRequest request = (HttpWebRequest)GetWebRequest(address);
            request.Method = WebRequestMethods.Http.Head;
            return request.GetResponseAsync();
        }

        public enum FileExistsHandling {
            Overwrite,
            Resume,
            Ignore,
            Rename,
        }

        public class DownloadPathChange {
            public Uri Uri { get; private set; }

            public string FullPath { get; set; }

            public DownloadPathChange(Uri uri) {
                Uri = uri;
            }
        }

        private class DownloadContext {
            public long CurrentFileLength { get; set; }
            public HttpWebRequest Request { get; set; }
            public HttpWebResponse Response { get; set; }
        }
    }
}
