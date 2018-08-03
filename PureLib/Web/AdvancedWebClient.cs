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
        public event EventHandler<EventArgs<RedirectContext>> Redirected;
        public event EventHandler<EventArgs<FileExistsContext>> FileExists;

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

            try {
                HttpWebResponse response = (HttpWebResponse)(await HeadAsync(address).ConfigureAwait(false));
                if (address != response.ResponseUri && Redirected != null) {
                    RedirectContext redirectContext = new RedirectContext(response.ResponseUri) {
                        FullPath = fullPath,
                    };
                    Redirected(this, new EventArgs<RedirectContext>(redirectContext));
                    fullPath = redirectContext.FullPath;
                }

                FileMode fileMode = FileMode.Create;
                FileInfo file = new FileInfo(fullPath);
                if (file.Exists) {
                    FileExistsContext fileExistsContext = new FileExistsContext(response.ResponseUri, fullPath) {
                        FileExistsHandling = _fileExistsHandling,
                    };
                    FileExists?.Invoke(this, new EventArgs<FileExistsContext>(fileExistsContext));

                    switch (fileExistsContext.FileExistsHandling) {
                        case FileExistsHandling.Ignore:
                            return;
                        case FileExistsHandling.Resume:
                            if (file.Length >= response.ContentLength)
                                return;

                            fileMode = FileMode.OpenOrCreate;
                            downloadContext.CurrentFileLength = file.Length;
                            break;
                        case FileExistsHandling.Rename:
                            do {
                                string newFileName = "{0} - New{1}".FormatWith(
                                    Path.GetFileNameWithoutExtension(fullPath), Path.GetExtension(fullPath));
                                fullPath = Path.Combine(Path.GetDirectoryName(fullPath), newFileName);
                                file = new FileInfo(fullPath);
                            } while (file.Exists);
                            break;
                    }
                }

                using (cancellationToken.Register(CancelAsync))
                using (Stream responseStream = await OpenReadTaskAsync(address).ConfigureAwait(false))
                using (FileStream fileStream = new FileStream(fullPath, fileMode)) {
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

        public class RedirectContext {
            public Uri Uri { get; private set; }

            public string FullPath { get; set; }

            public RedirectContext(Uri uri) {
                Uri = uri;
            }
        }

        public class FileExistsContext {
            public Uri Uri { get; private set; }
            public string FullPath { get; private set; }

            public FileExistsHandling FileExistsHandling { get; set; }

            public FileExistsContext(Uri uri, string fullPath) {
                Uri = uri;
                FullPath = fullPath;
            }
        }

        private class DownloadContext {
            public long CurrentFileLength { get; set; }
            public HttpWebRequest Request { get; set; }
            public HttpWebResponse Response { get; set; }
        }
    }
}
