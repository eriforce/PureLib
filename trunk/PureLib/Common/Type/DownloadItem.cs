using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace PureLib.Common {
    public class DownloadItem {
        public string Url { get; private set; }
        public string Referer { get; private set; }
        public CookieContainer Cookies { get; private set; }
        public string FilePath { get; private set; }

        public string FileName {
            get {
                return Path.GetFileName(FilePath);
            }
        }

        public virtual DownloadItemState State { get; set; }
        public virtual long TotalBytes { get; set; }
        public virtual long ReceivedBytes { get; set; }
        public virtual int Percentage { get; set; }

        public DownloadItem(string url, string referer, CookieContainer cookies, string path) {
            Url = url;
            Referer = referer;
            Cookies = cookies;
            FilePath = path;
        }
    }
}
