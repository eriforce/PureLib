using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PureLib.Properties;

namespace PureLib.Common {
    public static class ExceptionHandling {
        private const string tokenType = "Type";
        private const string tokenMessage = "Message";
        private const string tokenStack = "Stack";

        public static string GetTraceText(this Exception ex, string template = null, string innerExceptionSeparator = null, string token = Templating.Token) {
            Dictionary<string, string> tokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { tokenType, ex.GetType().AssemblyQualifiedName },
                { tokenMessage, ex.Message },
                { tokenStack, ex.StackTrace },
            };

            if (template == null)
                template = string.Join(Environment.NewLine, 
                    tokens.Keys.Select(k => "{0}{1}{0}".FormatWith(token, k)));

            if (innerExceptionSeparator == null)
                innerExceptionSeparator = "{0}{1}{0}".FormatWith(
                    Environment.NewLine, Resources.ExceptionHandling_InnerException);

            StringBuilder sb = new StringBuilder();
            while (ex != null) {
                if (sb.Length > 0)
                    sb.AppendLine(innerExceptionSeparator);
                sb.AppendLine(template.FillInTemplate(tokens));

                ex = ex.InnerException;
            }
            return sb.ToString();
        }
    }
}
