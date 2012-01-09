using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using PureLib.Common.Entities;

namespace PureLib.Common {
    /// <summary>
    /// Provides ability to download a list of items.
    /// </summary>
    public class WebDownloader {
        private Dictionary<AdvancedWebClient, DownloadItem> _clientItemMaps;
        private List<DownloadItem> _items;
        private int _downloadingCount;

        /// <summary>
        /// Indicates download thread count
        /// </summary>
        public int ThreadCount { get; private set; }

        /// <summary>
        /// Initializes a new instance of WebDownloader with one thread.
        /// </summary>
        public WebDownloader()
            : this(1) {
        }

        /// <summary>
        /// Initializes a new instance of WebDownloader with specified thread(s).
        /// </summary>
        /// <param name="threadCount"></param>
        public WebDownloader(int threadCount) {
            _clientItemMaps = new Dictionary<AdvancedWebClient, DownloadItem>();
            _items = new List<DownloadItem>();
            ThreadCount = threadCount;
        }

        /// <summary>
        /// Sets the thread count.
        /// </summary>
        /// <param name="count"></param>
        public void SetThreadCount(int count) {
            ThreadCount = count;
            StartDownloading();
        }

        /// <summary>
        /// Adds an item to download list.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(DownloadItem item) {
            _items.Add(item);
            if (item.State == DownloadItemState.Queued)
                StartDownloading();
        }

        private void StartDownloading() {
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
            item.State = DownloadItemState.Completed;
            _downloadingCount--;
            _clientItemMaps.Remove(client);
            StartDownloading();
        }
    }
}
