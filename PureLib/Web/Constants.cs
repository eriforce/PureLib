using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureLib.Web {
    public static class ContentType {
        public const string Form = "application/x-www-form-urlencoded";
        public const string Stream = "application/octet-stream";
        public const string Xml = "application/xml";
        public const string Json = "application/json";
    }

    public static class HttpHeader {
        public static class Request {
            public const string Authorization = "Authorization";
        }
        public static class Response {
            public const string Range = "Accept-Ranges";
            public const string ContentDisposition = "Content-Disposition";
        }
    }
}
