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

        public string Request(string url, string method, string param = null, string contentType = ContentType.Web) {
            return Request(new Uri(url), method, param, contentType);
        }

        public string Request(Uri uri, string method, string param = null, string contentType = ContentType.Web) {
            int retry = 0;
            while (retry <= RetryLimit) {
                try {
                    return RequestInternal(uri, method, param, contentType);
                }
                catch (WebException) {
                    retry++;
                    Thread.Sleep(RetryInterval);
                }
                catch (IOException) {
                    retry++;
                    Thread.Sleep(RetryInterval);
                }
            }
            Exception ex = new WebException("Request failed after {0} times retried.".FormatWith(RetryLimit));
            ex.Data.Add("Url", uri);
            throw ex;
        }

        private string RequestInternal(Uri uri, string method, string param, string contentType) {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            req.CookieContainer = Cookies;
            req.UserAgent = UserAgent;
            req.AllowAutoRedirect = AllowAutoRedirect;
            req.Referer = Referer;
            req.Method = method;
            req.ContentType = contentType;
            if (SetRequest != null)
                SetRequest(this, new EventArgs<HttpWebRequest>(req));

            if (!param.IsNullOrEmpty()) {
                byte[] buffer = Encoding.GetBytes(param);
                req.ContentLength = buffer.Length;
                using (Stream stream = req.GetRequestStream()) {
                    stream.Write(buffer, 0, buffer.Length);
                }
            }

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            if (GotResponse != null)
                GotResponse(this, new EventArgs<HttpWebResponse>(res));
            using (StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding)) {
                return sr.ReadToEnd();
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
