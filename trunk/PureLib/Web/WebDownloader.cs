using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using PureLib.Common;

namespace PureLib.Web {
    public class WebDownloader {
        private object _clientItemMapsLock;
        private Dictionary<IAsyncWebClient, DownloadItem> _clientItemMaps;
        private List<DownloadItem> _items;

        public bool UseResumableClient { get; private set; }
        public int ThreadCount { get; private set; }
        public bool IsStopped {
            get {
                lock (_clientItemMapsLock) {
                    return _clientItemMaps.Count == 0;
                }
            }
        }

        public event DownloadCompletingEventHandler DownloadCompleting;

        public WebDownloader(bool useResumableClient = false)
            : this(1, useResumableClient) {
        }

        public WebDownloader(int threadCount, bool useResumableClient) {
            CheckThreadCount(threadCount);

            UseResumableClient = useResumableClient;
            _clientItemMapsLock = new object();
            _clientItemMaps = new Dictionary<IAsyncWebClient, DownloadItem>();
            _items = new List<DownloadItem>();
            SetThreadCount(threadCount);
        }

        public void SetThreadCount(int threadCount) {
            CheckThreadCount(threadCount);

            ThreadCount = threadCount;
            StartDownloading();
        }

        public void AddItem(DownloadItem item) {
            AddItems(new List<DownloadItem>() { item });
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

        public void RemoveItem(DownloadItem item) {
            RemoveItems(new List<DownloadItem>() { item });
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
            foreach (DownloadItem i in _items.Where(i => i.IsReady)) {
                i.Stop();
            }
        }

        public void ResumeAllItems() {
            foreach (DownloadItem i in _items.Where(i => i.IsStopped)) {
                i.Start();
            }
            StartDownloading();
        }

        private void StartDownloading() {
            lock (_clientItemMapsLock) {
                if (_clientItemMaps.Count < ThreadCount) {
                    int needToStart = Math.Min(ThreadCount - _clientItemMaps.Count,
                        _items.Count(i => i.IsReady));
                    for (int i = 0; i < needToStart; i++)
                        Download();
                }
            }
        }

        private void ItemStateChanged(object sender, DownloadItemStateChangedEventArgs e) {
            switch (e.NewState) {
                case DownloadItemState.Queued:
                    lock (_clientItemMapsLock) {
                        Download();
                    }
                    break;
                case DownloadItemState.Stopped:
                    foreach (var p in _clientItemMaps) {
                        if (p.Value == e.DownloadItem) {
                            p.Key.CancelAsync();
                            break;
                        }
                    }
                    break;
            }
        }

        private void Download() {
            if (_clientItemMaps.Count < ThreadCount) {
                DownloadItem item = _items.FirstOrDefault(i => i.IsReady);
                if (item != null) {
                    item.Download();
                    object[] parameters = new object[] { item.Referer, item.UserName, item.Password, item.Cookies };
                    IAsyncWebClient client = File.Exists(item.FilePath) ?
                        (IAsyncWebClient)Utility.GetInstance<ResumableWebClient>(parameters) :
                        (IAsyncWebClient)Utility.GetInstance<AdvancedWebClient>(parameters);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);
                    if (client is AdvancedWebClient)
                        ((AdvancedWebClient)client).DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);
                    if (client is ResumableWebClient)
                        ((ResumableWebClient)client).RequestRangeNotSatisfiable += new EventHandler((s, e) => { DownloadFileCompleted(s, new AsyncCompletedEventArgs(null, false, null)); });
                    client.DownloadFileAsync(new Uri(item.Url), item.FilePath);
                    _clientItemMaps.Add(client, item);
                }
            }
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            DownloadItem item = _clientItemMaps[(IAsyncWebClient)sender];
            item.TotalBytes = e.TotalBytesToReceive;
            item.ReceivedBytes = e.BytesReceived;
            item.Percentage = e.ProgressPercentage;
        }

        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) {
            lock (_clientItemMapsLock) {
                dynamic client = sender;
                DownloadItem item = _clientItemMaps[client];
                _clientItemMaps.Remove(client);
                client.Dispose();
                if (e.Cancelled) {
                    item.Stop();
                    FileInfo file = new FileInfo(item.FilePath);
                    if (file.Exists) {
                        item.ReceivedBytes = file.Length;
                        if (item.TotalBytes > 0)
                            item.Percentage = (int)((item.ReceivedBytes * 100) / item.TotalBytes);
                    }
                }
                else {
                    if (IsDownloadedFileCorrupted(item))
                        item.Start();
                    else
                        item.Complete();
                    Download();
                }
            }
        }

        private void CheckThreadCount(int threadCount) {
            if (threadCount <= 0)
                throw new ArgumentOutOfRangeException("Thread count must be greater than zero.");
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
