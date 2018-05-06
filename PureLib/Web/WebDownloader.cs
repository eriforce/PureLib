using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PureLib.Common;

namespace PureLib.Web {
    public class WebDownloader {
        private object _clientsLock = new object();
        private List<DownloadItem> _items = new List<DownloadItem>();
        private Dictionary<DownloadItem, CancellationTokenSource> _downloadingItems = new Dictionary<DownloadItem, CancellationTokenSource>();

        public IWebProxy Proxy { get; private set; }
        public CookieContainer Cookies { get; private set; }
        public bool UseResumableClient { get; private set; }
        public int ThreadCount { get; private set; }
        public bool IsStopped {
            get { return _downloadingItems.Count == 0; }
        }

        public event DownloadCompletingEventHandler DownloadCompleting;

        public WebDownloader(bool useResumableClient = false)
            : this(1, null, null, useResumableClient) {
        }

        public WebDownloader(int threadCount, IWebProxy proxy, CookieContainer cookies, bool useResumableClient) {
            SetThreadCount(threadCount);
            Proxy = proxy;
            Cookies = cookies ?? new CookieContainer();
            UseResumableClient = useResumableClient;
        }

        public void SetThreadCount(int threadCount) {
            if (threadCount <= 0)
                throw new ArgumentOutOfRangeException("Thread count must be greater than zero.");

            ThreadCount = threadCount;
            StartDownloading();
        }

        public void AddItems(IEnumerable<DownloadItem> items) {
            if ((items == null) || items.Any(i => i == null))
                throw new ArgumentNullException("Download items are null.");

            foreach (DownloadItem item in items) {
                item.StateChanged += ItemStateChanged;
            }
            _items.AddRange(items);
            if (items.Any(i => i.IsReady))
                StartDownloading();
        }

        public void RemoveItems(IEnumerable<DownloadItem> items) {
            if ((items == null) || items.Any(i => i == null))
                throw new ArgumentNullException("Download items are null.");

            foreach (DownloadItem item in items) {
                item.Stop();
                item.StateChanged -= ItemStateChanged;
                _items.Remove(item);
            }
        }

        public void StopAllItems() {
            foreach (DownloadItem i in _items) {
                if (i.IsReady)
                    i.Stop();
            }
        }

        public void ResumeAllItems() {
            foreach (DownloadItem i in _items) {
                if (i.IsStopped)
                    i.Start();
            }
            StartDownloading();
        }

        private void StartDownloading() {
            lock (_clientsLock) {
                if (_downloadingItems.Count < ThreadCount) {
                    foreach (DownloadItem item in _items.Where(i => i.IsReady).Take(ThreadCount - _downloadingItems.Count)) {
                        _downloadingItems.Add(item, null);
                        DownloadAsync(item).ContinueWith(t => {
                            _downloadingItems.Remove(item);
                            StartDownloading();
                        });
                    }
                }
            }
        }

        private void ItemStateChanged(object sender, DownloadItemStateChangedEventArgs e) {
            switch (e.NewState) {
                case DownloadItemState.Queued:
                    StartDownloading();
                    break;
                case DownloadItemState.Stopped:
                    _downloadingItems[e.DownloadItem].Cancel();
                    break;
            }
        }

        private async Task DownloadAsync(DownloadItem item) {
            try {
                if (!Directory.Exists(item.Directory))
                    Directory.CreateDirectory(item.Directory);

                AdvancedWebClient client = UseResumableClient ? new ResumableWebClient() : new AdvancedWebClient();
                using (client) {
                    if (Proxy != null)
                        client.Proxy = Proxy;
                    client.DownloadProgressChanged += (s, e) => {
                        item.TotalBytes = e.TotalBytesToReceive;
                        item.ReceivedBytes = e.BytesReceived;
                    };
                    client.SetRequest += (s, e) => {
                        HttpWebRequest request = e.Data;
                        request.Referer = item.Referer;
                        request.CookieContainer = Cookies;
                        if (!item.UserName.IsNullOrEmpty() || !item.Password.IsNullOrEmpty()) {
                            request.Headers.Set(AdvancedWebClient.AuthorizationHeaderName,
                                AdvancedWebClient.GetBasicAuthenticationHeader(item.UserName, item.Password));
                        }
                    };

                    item.Download();
                    CancellationTokenSource source = new CancellationTokenSource();
                    _downloadingItems[item] = source;
                    await client.DownloadFileAsync(item.Uri, item.FilePath, source.Token).ConfigureAwait(false);
                }

                if (IsDownloadedFileCorrupted(item))
                    item.Start();
                else
                    item.Complete();
            }
            catch (OperationCanceledException) {
                FileInfo file = new FileInfo(item.FilePath);
                if (file.Exists)
                    item.ReceivedBytes = file.Length;
            }
            catch (Exception ex) {
                item.Error(ex);
            }
        }

        private bool IsDownloadedFileCorrupted(DownloadItem item) {
            DownloadCompletingEventArgs e = new DownloadCompletingEventArgs(item);
            DownloadCompleting?.Invoke(this, e);

            if (e.IsCorrupted)
                item.Error();
            return e.IsCorrupted;
        }
    }

    public class DownloadCompletingEventArgs : EventArgs {
        public bool IsCorrupted { get; set; }
        public DownloadItem Item { get; private set; }

        public DownloadCompletingEventArgs(DownloadItem item) {
            Item = item;
        }
    }
    public delegate void DownloadCompletingEventHandler(object sender, DownloadCompletingEventArgs e);
}
