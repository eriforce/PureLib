using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using PureLib.Common;

namespace PureLib.Web {
    public static class AdvancedWebClientExtensions {
        public const string AuthorizationHeaderName = "Authorization";

        public static void SetBasicAuthentication(this HttpWebRequest request, string username, string password) {
            var headerValue = "Basic {0}".FormatWith(Encoding.UTF8.GetBytes("{0}:{1}".FormatWith(username, password)).ToBase64String());
            request.Headers.Set(AuthorizationHeaderName, headerValue);
        }
    }
}
