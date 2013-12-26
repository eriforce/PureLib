using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using PureLib.Common;

namespace PureLib.Web {
    public class AdvancedWebClient : WebClient, IAsyncWebClient {
        public const string BasicAuthenticationHeaderName = "Authorization";

        private string _referer;
        private string _userName;
        private string _password;

        public CookieContainer Cookies { get; private set; }

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
            _referer = referer;
            _userName = userName;
            _password = password;
            Cookies = cookies;
        }

        public static string GetBasicAuthenticationHeader(string username, string password) {
            return "Basic {0}".FormatWith(Encoding.UTF8.GetBytes("{0}:{1}".FormatWith(username, password)).ToBase64String());
        }

        protected override WebRequest GetWebRequest(Uri address) {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.Referer = _referer;
            if (!_userName.IsNullOrEmpty() || !_password.IsNullOrEmpty()) {
                request.Credentials = new NetworkCredential(_userName, _password);
                request.Headers.Set(BasicAuthenticationHeaderName, GetBasicAuthenticationHeader(_userName, _password));
            }
            request.CookieContainer = Cookies;
            return request;
        }
    }
}
