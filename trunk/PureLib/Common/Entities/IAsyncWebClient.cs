using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PureLib.Common.Entities {
    public interface IAsyncWebClient {
        event AsyncCompletedEventHandler DownloadFileCompleted;

        void DownloadFileAsync(Uri address, string fileName);
        void CancelAsync();
    }
}
