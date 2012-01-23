﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using PureLib.Common.Entities;

namespace PureLib.Common {
    public class WebDownloader {
        private Dictionary<IAsyncWebClient, DownloadItem> _clientItemMaps;
        private List<DownloadItem> _items;

        public int ThreadCount { get; private set; }
        public bool IsStopped {
            get {
                return _clientItemMaps.Count == 0;
            }
        }

        public WebDownloader()
            : this(null, 1) {
        }

        public WebDownloader(int threadCount)
            : this(null, threadCount) {
        }

        public WebDownloader(List<DownloadItem> items, int threadCount) {
            CheckThreadCount(threadCount);

            _clientItemMaps = new Dictionary<IAsyncWebClient, DownloadItem>();
            _items = items ?? new List<DownloadItem>();
            foreach (DownloadItem item in _items) {
                item.StateChanged += ItemStateChanged;
            }
            SetThreadCount(threadCount);
        }

        public void SetThreadCount(int threadCount) {
            CheckThreadCount(threadCount);

            ThreadCount = threadCount;
            StartDownloading();
        }

        public void AddItem(DownloadItem item) {
            if (item == null)
                throw new ArgumentNullException("Download item is null.");

            item.StateChanged += ItemStateChanged;
            _items.Add(item);
            if (item.State == DownloadItemState.Queued)
                StartDownloading();
        }

        public void AddItems(List<DownloadItem> items) {
            if (items == null)
                throw new ArgumentNullException("Download items are null.");

            foreach (DownloadItem item in items) {
                item.StateChanged += ItemStateChanged;
            }
            _items.AddRange(items);
            if (items.Any(i => i.State == DownloadItemState.Queued))
                StartDownloading();
        }

        public void StopAllItems() {
            foreach (DownloadItem i in _items.Where(i => i.State == DownloadItemState.Queued)) {
                i.State = DownloadItemState.Stopped;
            }
        }

        public void ResumeAllItems() {
            foreach (DownloadItem i in _items.Where(i => i.State == DownloadItemState.Stopped)) {
                i.State = DownloadItemState.Queued;
            }
            StartDownloading();
        }

        private void StartDownloading() {
            if (_clientItemMaps.Count < ThreadCount) {
                int needToStart = Math.Min(ThreadCount - _clientItemMaps.Count,
                    _items.Count(i => i.State == DownloadItemState.Queued));
                for (int i = 0; i < needToStart; i++)
                    Download();
            }
        }

        private void ItemStateChanged(object sender, DownloadItemStateChangedEventArgs e) {
            switch (e.NewState) {
                case DownloadItemState.Queued:
                    Download();
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
            lock (this) {
                if (_clientItemMaps.Count < ThreadCount) {
                    DownloadItem item = _items.FirstOrDefault(i => i.State == DownloadItemState.Queued);
                    if (item != null) {
                        item.State = DownloadItemState.Downloading;

                        object[] parameters = new object[] { item.Referer, item.UserName, item.Password, item.Cookies };
                        IAsyncWebClient client = File.Exists(item.FilePath) ?
                            (IAsyncWebClient)Utility.GetInstance<ResumableWebClient>(parameters) :
                            (IAsyncWebClient)Utility.GetInstance<AdvancedWebClient>(parameters);
                        _clientItemMaps.Add(client, item);
                        client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);
                        if (client is AdvancedWebClient)
                            ((AdvancedWebClient)client).DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);
                        client.DownloadFileAsync(new Uri(item.Url), item.FilePath);
                    }
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
            dynamic client = sender;
            DownloadItem item = _clientItemMaps[client];
            _clientItemMaps.Remove(client);
            if (e.Cancelled) {
                item.State = DownloadItemState.Stopped;
                FileInfo file = new FileInfo(item.FilePath);
                if (file.Exists) {
                    item.ReceivedBytes = file.Length;
                    if (item.TotalBytes > 0)
                        item.Percentage = (int)((item.ReceivedBytes * 100) / item.TotalBytes);
                }
            }
            else {
                item.State = DownloadItemState.Completed;
                if (item.TotalBytes == 0)
                    item.TotalBytes = new FileInfo(item.FilePath).Length;
                item.ReceivedBytes = item.TotalBytes;
                item.Percentage = 100;
                Download();
            }
            client.Dispose();
        }

        private static void CheckThreadCount(int threadCount) {
            if (threadCount <= 0)
                throw new ArgumentOutOfRangeException("Thread count must be greater than zero.");
        }
    }
}
