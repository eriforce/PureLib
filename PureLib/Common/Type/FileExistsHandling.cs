using System;
using System.Collections.Generic;
using System.Text;

namespace PureLib.Common {
    public enum FileExistsHandling {
        Overwrite,
        Resume,
        Ignore,
        Rename,
    }
}
