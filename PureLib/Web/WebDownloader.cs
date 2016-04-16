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

        public CookieContainer Cookies { get; private set; }
        public bool UseResumableClient { get; private set; }
        public int ThreadCount { get; private set; }
        public bool IsStopped {
            get { return _downloadingItems.Count == 0; }
        }

        public event DownloadCompletingEventHandler DownloadCompleting;

        public WebDownloader(bool useResumableClient = false)
            : this(1, null, useResumableClient) {
        }

        public WebDownloader(int threadCount, CookieContainer cookies, bool useResumableClient) {
            SetThreadCount(threadCount);
            Cookies = cookies ?? new CookieContainer();
            UseResumableClient = useResumableClient;
        }

        public void SetThreadCount(int threadCount) {
            if (threadCount <= 0)
                throw new ArgumentOutOfRangeException("Thread count must be greater than zero.");

            ThreadCount = threadCount;
            StartDownloading();
        }

        public void AddItems(List<DownloadItem> items) {
            if ((items == null) || items.Any(i => i == null))
                throw new ArgumentNullException("Download items are null.");

            foreach (DownloadItem item in items) {
                item.StateChanged += ItemStateChanged;
            }
            _items.AddRange(items);
            if (items.Any(i => i.IsReady))
                StartDownloading();
        }

        public void RemoveItems(List<DownloadItem> items) {
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
                        Task.Run(() => DownloadAsync(item).ContinueWith(t => {
                            _downloadingItems.Remove(item);
                            StartDownloading();
                        }));
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
                if (!Directory.Exists(item.Location))
                    Directory.CreateDirectory(item.Location);

                AdvancedWebClient client = UseResumableClient ? new ResumableWebClient() : new AdvancedWebClient();
                using (client) {
                    client.DownloadProgressChanged += (s, e) => {
                        item.TotalBytes = e.TotalBytesToReceive;
                        item.ReceivedBytes = e.BytesReceived;
                        item.Percentage = e.ProgressPercentage;
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
                    await client.DownloadFileAsync(item.Url, item.FilePath, source.Token).ConfigureAwait(false);
                }

                if (IsDownloadedFileCorrupted(item))
                    item.Start();
                else
                    item.Complete();
            }
            catch (OperationCanceledException) {
                FileInfo file = new FileInfo(item.FilePath);
                if (file.Exists) {
                    item.ReceivedBytes = file.Length;
                    if (item.TotalBytes > 0)
                        item.Percentage = (int)((item.ReceivedBytes * 100) / item.TotalBytes);
                }
            }
            catch (Exception) {
                item.Error();
            }
        }

        private bool IsDownloadedFileCorrupted(DownloadItem item) {
            DownloadCompletingEventArgs e = new DownloadCompletingEventArgs(item);
            if (DownloadCompleting != null)
                DownloadCompleting(this, e);
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
