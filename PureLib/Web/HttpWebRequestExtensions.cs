using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PureLib.Common;

namespace PureLib.Web {
    public static class AdvancedWebClientExtensions {
        public static void SetBasicAuthentication(this HttpWebRequest request, string username, string password) {
            var headerValue = "Basic {0}".FormatWith(Encoding.UTF8.GetBytes("{0}:{1}".FormatWith(username, password)).ToBase64String());
            request.Headers.Set(HttpHeader.Request.Authorization, headerValue);
        }

        public static bool IsRangeSupported(this HttpWebResponse response) {
            string rangeValue = response.GetResponseHeader(HttpHeader.Response.Range);
            return !rangeValue.IsNullOrEmpty() && !"none".Equals(rangeValue, StringComparison.OrdinalIgnoreCase);
        }

        public static async Task<bool> IsRangeSupported(this HttpWebRequest request) {
            string method = request.Method;

            try {
                request.Method = WebRequestMethods.Http.Head;
                HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync().ConfigureAwait(false));
                return IsRangeSupported(response);
            }
            finally {
                request.Method = method;
            }
        }
    }
}
