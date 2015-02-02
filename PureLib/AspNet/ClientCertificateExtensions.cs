using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using PureLib.Common;

namespace PureLib.AspNet {
    public static class ClientCertificateExtensions {
        public static bool CertAuth(this HttpRequestBase request, string issuer = null) {
            if (Debugger.IsAttached)
                FormsAuthentication.SetAuthCookie("Debug", false, request.ApplicationPath);

            HttpClientCertificate cert = request.ClientCertificate;
            if (ValidateClientCertificate(cert, issuer))
                FormsAuthentication.SetAuthCookie(cert.Subject.Substring("CN=".Length), false, request.ApplicationPath);
            else
                return false;

            return true;
        }

        private static bool ValidateClientCertificate(HttpClientCertificate cert, string issuer) {
            if ((cert == null) || !cert.IsValid)
                return false;

            if (!issuer.IsNullOrEmpty() && !cert.BinaryIssuer.ToHexString().Equals(issuer, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
