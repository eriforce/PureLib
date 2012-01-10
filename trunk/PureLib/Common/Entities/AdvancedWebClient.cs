using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace PureLib.Common.Entities {
    public class AdvancedWebClient : WebClient {
        private string referer;
        private CookieContainer cookies;

        public AdvancedWebClient()
            : this(null, null) {
        }

        public AdvancedWebClient(string referer)
            : this(referer, null) {
        }

        public AdvancedWebClient(CookieContainer cookies)
            : this(null, cookies) {
        }

        public AdvancedWebClient(string referer, CookieContainer cookies) {
            this.referer = referer;
            this.cookies = cookies;
        }

        protected override WebRequest GetWebRequest(Uri address) {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.Referer = referer;
            request.CookieContainer = cookies;
            return request;
        }
    }
}
