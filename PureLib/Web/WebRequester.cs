using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

        public Task<(bool, string)> GetAsync(string url) {
            return RequestAsync(url, HttpMethod.Get);
        }

        public Task<(bool, string)> PostAsync(string url, string param, string contentType = ContentTypes.Form) {
            return RequestAsync(url, HttpMethod.Post, param, contentType);
        }

        public Task<(bool, string)> PutAsync(string url, string param, string contentType = ContentTypes.Form) {
            return RequestAsync(url, HttpMethod.Put, param, contentType);
        }

        public Task<(bool, string)> DeleteAsync(string url) {
            return RequestAsync(url, HttpMethod.Delete);
        }

        public Task<(bool, string)> RequestAsync(string url, HttpMethod method, string param = null, string contentType = ContentTypes.Form) {
            return RequestAsync(new Uri(url), method, param, contentType);
        }

        public Task<(bool, string)> RequestAsync(Uri uri, HttpMethod method, string param = null, string contentType = ContentTypes.Form) {
            return RequestAsync(uri, method, (param == null) ? null : Encoding.GetBytes(param), contentType);
        }

        public Task<(bool, string)> RequestAsync(string url, HttpMethod method, byte[] data, string contentType = ContentTypes.Stream) {
            return RequestAsync(new Uri(url), method, data, contentType);
        }

        public async Task<(bool, string)> RequestAsync(Uri uri, HttpMethod method, byte[] data, string contentType = ContentTypes.Stream) {
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

        private async Task<(bool, string)> RequestInternalAsync(Uri uri, HttpMethod httpMethod, byte[] data, string contentType) {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            req.CookieContainer = Cookies;
            req.UserAgent = UserAgent;
            req.AllowAutoRedirect = AllowAutoRedirect;
            req.Referer = Referer;
            req.Method = httpMethod.Method;
            req.ContentType = contentType;
            SetRequest?.Invoke(this, new EventArgs<HttpWebRequest>(req));

            if (data != null && data.Any() && (httpMethod != HttpMethod.Get) && (httpMethod != HttpMethod.Head)) {
                req.ContentLength = data.Length;
                using (Stream stream = await req.GetRequestStreamAsync().ConfigureAwait(false)) {
                    await stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
                }
            }

            HttpWebResponse res = (HttpWebResponse)(await req.GetResponseAsync().ConfigureAwait(false));
            GotResponse?.Invoke(this, new EventArgs<HttpWebResponse>(res));
            Stream responseStream = res.GetResponseStream();
            using (StreamReader sr = new StreamReader(responseStream, Encoding)) {
                string response = await sr.ReadToEndAsync().ConfigureAwait(false);
                return (IsSuccessStatusCode((int)res.StatusCode), response);
            }
        }

        private bool IsSuccessStatusCode(int statusCode) {
            return (statusCode >= 200) && (statusCode <= 299);
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
