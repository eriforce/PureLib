using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PureLib.Common {
    public enum DownloadItemState {
        Stopped,
        Downloading,
        Queued,
        Completed,
        Error,
    }
}
