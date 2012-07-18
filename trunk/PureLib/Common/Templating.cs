using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PureLib.Common {
    public static class Templating {
        public const string Token = "%%";

        public static string FillInTemplate(this string template, Dictionary<string, string> tokens, string templateToken = Token) {
            string[] parts = template.Split(new string[] { templateToken }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            foreach (string part in parts) {
                string value;
                if (tokens.TryGetValue(part, out value))
                    sb.Append(value);
                else
                    sb.Append(part);
            }
            return sb.ToString();
        }
    }
}
