using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace PureLib.Common.Entities {
    public class AdvancedWebClient : WebClient, IAsyncWebClient {
        public const string BasicAuthenticationHeaderName = "Authorization";

        private string referer;
        private string userName;
        private string password;
        private CookieContainer cookies;

        public AdvancedWebClient()
            : this(null, null, null, null) {
        }

        public AdvancedWebClient(string referer)
            : this(referer, null, null, null) {
        }

        public AdvancedWebClient(string userName, string password)
            : this(null, userName, password, null) {
        }

        public AdvancedWebClient(CookieContainer cookies)
            : this(null, null, null, cookies) {
        }

        public AdvancedWebClient(string referer, string userName, string password, CookieContainer cookies) {
            this.referer = referer;
            this.userName = userName;
            this.password = password;
            this.cookies = cookies;
        }

        public static string GetBasicAuthenticationHeader(string username, string password)
        {
            return "Basic {0}".FormatWith(Encoding.UTF8.GetBytes("{0}:{1}".FormatWith(username, password)).ToBase64String());
        }


        protected override WebRequest GetWebRequest(Uri address) {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.Referer = referer;
            if (!userName.IsNullOrEmpty() || !password.IsNullOrEmpty()) {
                request.Credentials = new NetworkCredential(userName, password);
                request.Headers.Set(BasicAuthenticationHeaderName, GetBasicAuthenticationHeader(userName, password));
            }
            request.CookieContainer = cookies;
            return request;
        }
    }
}
