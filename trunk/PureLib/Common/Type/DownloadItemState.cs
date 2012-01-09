using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PureLib.Common {
    /// <summary>
    /// Specifies the state of DownloadItem.
    /// </summary>
    public enum DownloadItemState {
        /// <summary>
        /// Indicates item is stopped.
        /// </summary>
        Stopped,
        /// <summary>
        /// Indicates item is downloading.
        /// </summary>
        Downloading,
        /// <summary>
        /// Indicates item is queued.
        /// </summary>
        Queued,
        /// <summary>
        /// Indicates item is completed.
        /// </summary>
        Completed,
        /// <summary>
        /// Indicates error occurred while downloading.
        /// </summary>
        Error,
    }
}
