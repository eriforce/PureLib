using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using PureLib.Common;

namespace PureLib.Web {
    public static class HttpWebExtensions {
        public static void SetBasicAuthentication(this HttpWebRequest request, string username, string password) {
            var credential = Convert.ToBase64String(Encoding.UTF8.GetBytes("{0}:{1}".FormatWith(username, password)));
            request.Headers.Set(HttpHeaders.Request.Authorization, $"Basic {credential}");
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

        public static bool IsRangeSupported(this HttpWebResponse response) {
            string rangeValue = response.GetResponseHeader(HttpHeaders.Response.Range);
            return !rangeValue.IsNullOrEmpty() && !"none".Equals(rangeValue, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetContentDispositionFileName(this HttpWebResponse response) {
            string disposition = response.GetResponseHeader(HttpHeaders.Response.ContentDisposition);
            if (disposition.IsNullOrEmpty())
                return null;

            ContentDispositionHeaderValue value = ContentDispositionHeaderValue.Parse(disposition);
            return !value.FileNameStar.IsNullOrEmpty() ? value.FileNameStar : value.FileName;
        }
    }
}
