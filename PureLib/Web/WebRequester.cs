using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using PureLib.Common;

namespace PureLib.Web {
    public class WebRequester {
        public CookieContainer Cookies { get; private set; }

        public string UserAgent { get; set; }
        public string Referer { get; set; }
        public bool AllowAutoRedirect { get; set; }
        public Encoding Encoding { get; set; }
        public int RetryInterval { get; set; }
        public int RetryLimit { get; set; }

        public event EventHandler<EventArgs<HttpWebRequest>> SetRequest;
        public event EventHandler<EventArgs<HttpWebResponse>> GotResponse;

        public WebRequester()
            : this(new CookieContainer()) {
        }

        public WebRequester(CookieContainer cookies) {
            Cookies = cookies;

            UserAgent = GetUserAgent();
            AllowAutoRedirect = false;
            Encoding = Encoding.UTF8;
            RetryInterval = 1000;
            RetryLimit = int.MaxValue;
        }

        public Task<string> GetAsync(string url) {
            return RequestAsync(url, WebRequestMethods.Http.Get);
        }

        public Task<string> PostAsync(string url, string param, string contentType = ContentType.Form) {
            return RequestAsync(url, WebRequestMethods.Http.Post, param, contentType);
        }

        public Task<string> PutAsync(string url, string param, string contentType = ContentType.Form) {
            return RequestAsync(url, WebRequestMethods.Http.Put, param, contentType);
        }

        public Task<string> DeleteAsync(string url) {
            return RequestAsync(url, "DELETE");
        }

        public Task<string> RequestAsync(string url, string method, string param = null, string contentType = ContentType.Form) {
            return RequestAsync(new Uri(url), method, param, contentType);
        }

        public Task<string> RequestAsync(Uri uri, string method, string param = null, string contentType = ContentType.Form) {
            return RequestAsync(uri, method, (param == null) ? null : Encoding.GetBytes(param), contentType);
        }

        public Task<string> RequestAsync(string url, string method, byte[] data, string contentType = ContentType.Stream) {
            return RequestAsync(new Uri(url), method, data, contentType);
        }

        public async Task<string> RequestAsync(Uri uri, string method, byte[] data, string contentType = ContentType.Stream) {
            int retry = 0;
            while (retry <= RetryLimit) {
                try {
                    return await RequestInternalAsync(uri, method, data, contentType).ConfigureAwait(false);
                }
                catch (WebException) {
                    retry++;
                    await Task.Delay(RetryInterval).ConfigureAwait(false);
                }
                catch (IOException) {
                    retry++;
                    await Task.Delay(RetryInterval).ConfigureAwait(false);
                }
            }
            Exception ex = new WebException("Request failed after retried {0} times.".FormatWith(RetryLimit));
            ex.Data.Add("Url", uri);
            throw ex;
        }

        private async Task<string> RequestInternalAsync(Uri uri, string method, byte[] data, string contentType) {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            req.CookieContainer = Cookies;
            req.UserAgent = UserAgent;
            req.AllowAutoRedirect = AllowAutoRedirect;
            req.Referer = Referer;
            req.Method = method;
            req.ContentType = contentType;
            SetRequest?.Invoke(this, new EventArgs<HttpWebRequest>(req));

            if ((data != null) && data.Any() && (method != WebRequestMethods.Http.Get) && (method != WebRequestMethods.Http.Head)) {
                req.ContentLength = data.Length;
                using (Stream stream = req.GetRequestStream()) {
                    await stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
                }
            }

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            GotResponse?.Invoke(this, new EventArgs<HttpWebResponse>(res));
            Stream responseStream = res.GetResponseStream();
            using (StreamReader sr = new StreamReader(responseStream, Encoding)) {
                return await sr.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        private string GetUserAgent() {
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            return "Mozilla/5.0 ({0} {1}; {2}) {3}/{4}".FormatWith(
                Environment.OSVersion.Platform,
                Environment.OSVersion.Version.ToString(2),
                Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"),
                assemblyName.Name,
                assemblyName.Version);
        }
    }
}
