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

        public event EventHandler<EventArgs<HttpWebRequest>> SetRequest;
        public event EventHandler<EventArgs<HttpWebResponse>> GotResponse;

        public async Task DownloadFileAsync(Uri address, string path, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            FileInfo file = new FileInfo(path);
            long fileLength = !file.Exists ? 0 : file.Length;
            if (!_uriContexts.TryAdd(address, new DownloadContext { FileLength = fileLength }))
                throw new ApplicationException("The url is downloading.");

            try {
                using (cancellationToken.Register(CancelAsync))
                using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
                using (Stream responseStream = await OpenReadTaskAsync(address).ConfigureAwait(false)) {
                    bool isRangeSupported = _uriContexts[address].Response.IsRangeSupported();
                    if (isRangeSupported)
                        fileStream.Seek(fileLength, SeekOrigin.Begin);

                    await responseStream.CopyToAsync(fileStream).ConfigureAwait(false);
                    await fileStream.FlushAsync().ConfigureAwait(false);
                }
            }
            finally {
                DownloadContext value;
                _uriContexts.TryRemove(address, out value);
            }
        }

        protected override WebRequest GetWebRequest(Uri address) {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            SetRequest?.Invoke(this, new EventArgs<HttpWebRequest>(request));

            long length = _uriContexts[address].FileLength;
            if (length > 0)
                request.AddRange(length);

            _uriContexts[address].Request = request;

            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request) {
            HttpWebResponse response = (HttpWebResponse)base.GetWebResponse(request);
            GotResponse?.Invoke(this, new EventArgs<HttpWebResponse>(response));

            _uriContexts[request.RequestUri].Response = response;

            return response;
        }

        private class DownloadContext {
            public long FileLength { get; set; }
            public HttpWebRequest Request { get; set; }
            public HttpWebResponse Response { get; set; }
        }
    }
}
