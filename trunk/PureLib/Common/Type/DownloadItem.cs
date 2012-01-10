using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace PureLib.Common {
    public class DownloadItem {
        public string Url { get; private set; }
        public string Referer { get; private set; }
        public CookieContainer Cookies { get; private set; }
        public string Path { get; private set; }

        public virtual DownloadItemState State { get; set; }
        public virtual long TotalBytes { get; set; }
        public virtual long ReceivedBytes { get; set; }

        public DownloadItem(string url, string referer, CookieContainer cookies, string path) {
            Url = url;
            Referer = referer;
            Cookies = cookies;
            Path = path;
        }
    }
}
