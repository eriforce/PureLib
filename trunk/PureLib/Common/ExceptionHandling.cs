using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PureLib.Properties;

namespace PureLib.Common {
    public static class ExceptionHandling {
        public static string GetTraceText(this Exception ex) {
            StringBuilder sb = new StringBuilder();
            while (ex != null) {
                if (sb.Length > 0) {
                    sb.AppendLine()
                        .AppendLine(Resources.ExceptionHandling_InnerException)
                        .AppendLine();
                }
                sb.AppendLine(ex.Message)
                    .AppendLine(ex.StackTrace);

                ex = ex.InnerException;
            }
            return sb.ToString();
        }
    }
}
