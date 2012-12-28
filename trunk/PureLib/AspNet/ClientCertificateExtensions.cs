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
        public static bool Authenticate(this HttpClientCertificate certificate, string issuer = null) {
            if (Debugger.IsAttached)
                FormsAuthentication.SetAuthCookie("Debug", false);

            if (ValidateClientCertificate(certificate, issuer))
                FormsAuthentication.SetAuthCookie(certificate.Subject.Substring("CN=".Length), false);
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
