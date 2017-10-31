using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PureLib.Common;

namespace PureLib.Web {
    public class AdvancedWebClient : WebClient {
        public const string AuthorizationHeaderName = "Authorization";

        public event EventHandler<EventArgs<HttpWebRequest>> SetRequest;

        public virtual async Task DownloadFileAsync(Uri address, string fileName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            using (cancellationToken.Register(CancelAsync)) {
                await DownloadFileTaskAsync(address, fileName).ConfigureAwait(false);
            }
        }

        protected override WebRequest GetWebRequest(Uri address) {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            SetRequest?.Invoke(this, new EventArgs<HttpWebRequest>(request));
            return request;
        }

        public static string GetBasicAuthenticationHeader(string username, string password) {
            return "Basic {0}".FormatWith(Encoding.UTF8.GetBytes("{0}:{1}".FormatWith(username, password)).ToBase64String());
        }
    }
}
