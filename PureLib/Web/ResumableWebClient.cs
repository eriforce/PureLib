using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PureLib.Common;

namespace PureLib.Web {
    public sealed class ResumableWebClient : AdvancedWebClient {
        private string _fileName;

        // TODO: DownloadFileAsync is not implemented correctly, the file exists will be overwritten with partial content
        public override Task DownloadFileAsync(Uri address, string fileName, CancellationToken cancellationToken) {
            throw new NotImplementedException();

            _fileName = fileName;
            return base.DownloadFileAsync(address, fileName, cancellationToken);
        }

        protected override WebRequest GetWebRequest(Uri address) {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            FileInfo file = new FileInfo(_fileName);
            if (file.Exists)
                request.AddRange(file.Length);
            return request;
        }
    }
}
