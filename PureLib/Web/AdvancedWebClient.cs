using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PureLib.Common;

namespace PureLib.Web {
    public class AdvancedWebClient : WebClient {
        public event EventHandler<EventArgs<HttpWebRequest>> SetRequest;

        public Task DownloadFileAsync(Uri address, string path, FileMode fileMode) {
            return DownloadFileAsync(address, path, fileMode, CancellationToken.None);
        }

        public async Task DownloadFileAsync(Uri address, string path, FileMode fileMode, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            using (cancellationToken.Register(CancelAsync))
            using (FileStream fileStream = new FileStream(path, fileMode))
            using (Stream responseStream = await OpenReadTaskAsync(address).ConfigureAwait(false)) {
                await responseStream.CopyToAsync(fileStream).ConfigureAwait(false);
                await fileStream.FlushAsync().ConfigureAwait(false);
            }
        }

        protected override WebRequest GetWebRequest(Uri address) {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            SetRequest?.Invoke(this, new EventArgs<HttpWebRequest>(request));
            return request;
        }
    }
}
