using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;
using PureLib.Common;

namespace PureLib.Common {
    public static class Serializer {
        public static string ToJson(this object obj, bool indent = true) {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(obj);
            if (indent)
                return new JsonFormatter().FormatJson(json);
            else
                return json;
        }

        public static T FromJson<T>(this string json) {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(json);
        }
    }
}