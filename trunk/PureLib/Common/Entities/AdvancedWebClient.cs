using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace PureLib.Common.Entities {
    /// <summary>
    /// A advanced WebClient supports cookie and referer on HTTP request.
    /// </summary>
    public class AdvancedWebClient : WebClient {
        private string referer;
        private CookieContainer cookies;

        /// <summary>
        /// Initializes a new instance of AdvancedWebClient.
        /// </summary>
        public AdvancedWebClient()
            : this(null, null) {
        }

        /// <summary>
        /// Initializes a new instance of AdvancedWebClient with specified referer.
        /// </summary>
        /// <param name="referer">Referer on HTTP request</param>
        public AdvancedWebClient(string referer)
            : this(referer, null) {
        }

        /// <summary>
        /// Initializes a new instance of AdvancedWebClient with specified cookies.
        /// </summary>
        /// <param name="cookies">Cookies on HTTP request</param>
        public AdvancedWebClient(CookieContainer cookies)
            : this(null, cookies) {
        }

        /// <summary>
        /// Initializes a new instance of AdvancedWebClient with specified referer and cookies.
        /// </summary>
        /// <param name="referer">Referer on HTTP request</param>
        /// <param name="cookies">Cookies on HTTP request</param>
        public AdvancedWebClient(string referer, CookieContainer cookies) {
            this.referer = referer;
            this.cookies = cookies;
        }

        /// <summary>
        /// Returns a WebRequest object for the specified resource.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address) {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.Referer = referer;
            request.CookieContainer = cookies;
            return request;
        }
    }
}
