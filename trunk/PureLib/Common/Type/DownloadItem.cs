using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace PureLib.Common {
    /// <summary>
    /// A structure of item to be downloaded.
    /// </summary>
    public class DownloadItem {
        /// <summary>
        /// Url of HTTP request
        /// </summary>
        public string Url { get; private set; }
        /// <summary>
        /// Referer of HTTP request
        /// </summary>
        public string Referer { get; private set; }
        /// <summary>
        /// Cookies of HTTP request
        /// </summary>
        public CookieContainer Cookies { get; private set; }
        /// <summary>
        /// Local path to store data
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Current state
        /// </summary>
        public virtual DownloadItemState State { get; set; }
        /// <summary>
        /// Data size
        /// </summary>
        public virtual long TotalBytes { get; set; }
        /// <summary>
        /// Downloaded data size
        /// </summary>
        public virtual long ReceivedBytes { get; set; }

        /// <summary>
        /// Initializes a new instance of DownloadItem.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="referer"></param>
        /// <param name="cookies"></param>
        /// <param name="path"></param>
        public DownloadItem(string url, string referer, CookieContainer cookies, string path) {
            Url = url;
            Referer = referer;
            Cookies = cookies;
            Path = path;
        }
    }
}
