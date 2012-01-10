using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using PureLib.Common.Entities;

namespace PureLib.Common {
    public class WebDownloader {
        private Dictionary<AdvancedWebClient, DownloadItem> _clientItemMaps;
        private List<DownloadItem> _items;
        private int _downloadingCount;

        public int ThreadCount { get; private set; }
        public bool IsStopped { get; private set; }

        public WebDownloader()
            : this(null, 1) {
        }

        public WebDownloader(int threadCount)
            : this(null, threadCount) {
        }

        public WebDownloader(List<DownloadItem> items, int threadCount) {
            _clientItemMaps = new Dictionary<AdvancedWebClient, DownloadItem>();
            _items = items ?? new List<DownloadItem>();
            StartDownloading(threadCount);
        }

        public void StartDownloading(int threadCount) {
            if (threadCount <= 0)
                throw new ArgumentOutOfRangeException("Thread count must be greater than zero.");

            ThreadCount = threadCount;
            for (int i = 0; i < ThreadCount; i++)
                Download();
        }

        public void AddItem(DownloadItem item) {
            if (item == null)
                throw new ArgumentNullException("Download item is null.");

            _items.Add(item);
            if (item.State == DownloadItemState.Queued)
                Download();
        }

        public void StopAll() {
            foreach (DownloadItem i in _items.Where(i => i.State == DownloadItemState.Queued)) {
                i.State = DownloadItemState.Stopped;
            }
            foreach (var p in _clientItemMaps) {
                p.Key.CancelAsync();
            }
            IsStopped = true;
        }

        public void ResumeAll() {
            foreach (DownloadItem i in _items.Where(i => i.State == DownloadItemState.Stopped)) {
                i.State = DownloadItemState.Queued;
            }
            StartDownloading(ThreadCount);
            IsStopped = false;
        }

        private void Download() {
            lock (this) {
                if (_downloadingCount < ThreadCount) {
                    DownloadItem item = _items.FirstOrDefault(i => i.State == DownloadItemState.Queued);
                    if (item != null) {
                        item.State = DownloadItemState.Downloading;
                        _downloadingCount++;

                        using (AdvancedWebClient client = new AdvancedWebClient(item.Referer, item.Cookies)) {
                            _clientItemMaps.Add(client, item);
                            client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);
                            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);
                            client.DownloadFileAsync(new Uri(item.Url), item.Path);
                        }
                    }
                }
            }
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            DownloadItem item = _clientItemMaps[(AdvancedWebClient)sender];
            item.TotalBytes = e.TotalBytesToReceive;
            item.ReceivedBytes = e.BytesReceived;
        }

        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) {
            AdvancedWebClient client = (AdvancedWebClient)sender;
            DownloadItem item = _clientItemMaps[client];
            _downloadingCount--;
            if (e.Cancelled)
                item.State = DownloadItemState.Stopped;
            else {
                item.State = DownloadItemState.Completed;
                Download();
            }
            _clientItemMaps.Remove(client);
        }
    }
}
