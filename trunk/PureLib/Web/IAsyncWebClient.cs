using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PureLib.Web {
    public interface IAsyncWebClient {
        event AsyncCompletedEventHandler DownloadFileCompleted;

        void DownloadFileAsync(Uri address, string fileName);
        void CancelAsync();
    }
}
