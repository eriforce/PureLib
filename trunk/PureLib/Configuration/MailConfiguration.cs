using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace PureLib.Configuration {
    /// <summary>
    /// Represents a mail element within a configuration file.
    /// </summary>
    public class MailConfiguration : ConfigurationElement {
        private const string from = "from";
        private const string to = "to";
        private const string cc = "cc";
        private const string bcc = "bcc";
        private const string host = "host";
        private const string port = "port";
        private const string ssl = "ssl";
        private const string userName = "username";
        private const string password = "password";

        /// <summary>
        /// Specifies the from address for e-mails.
        /// </summary>
        [ConfigurationProperty(from, IsRequired = true)]
        public string From {
            get { return (string)this[from]; }
            set { this[from] = value; }
        }

        /// <summary>
        /// Specifies the to addresses for e-mails.
        /// </summary>
        [ConfigurationProperty(to, IsRequired = true)]
        public string To {
            get { return (string)this[to]; }
            set { this[to] = value; }
        }

        /// <summary>
        /// Specifies the carbon copy addresses for e-mails.
        /// </summary>
        [ConfigurationProperty(cc)]
        public string CC {
            get { return (string)this[cc]; }
            set { this[cc] = value; }
        }

        /// <summary>
        /// Specifies the blind carbon copy addresses for e-mails.
        /// </summary>
        [ConfigurationProperty(bcc)]
        public string Bcc {
            get { return (string)this[bcc]; }
            set { this[bcc] = value; }
        }

        /// <summary>
        /// Specifies the hostname of the SMTP mail server to use for SMTP transactions.
        /// </summary>
        [ConfigurationProperty(host, IsRequired = true)]
        public string Host {
            get { return (string)this[host]; }
            set { this[host] = value; }
        }

        /// <summary>
        /// Specifies the port number to use to connect to the SMTP mail server. The default value is 25.
        /// </summary>
        [ConfigurationProperty(port, DefaultValue = 25, IsRequired = true)]
        public int Port {
            get { return (int)this[port]; }
            set { this[port] = value; }
        }

        /// <summary>
        /// Specifies whether SSL is used to access an SMTP mail server. The default value is false.
        /// </summary>
        [ConfigurationProperty(ssl, DefaultValue = false, IsRequired = true)]
        public bool EnableSsl {
            get { return (bool)this[ssl]; }
            set { this[ssl] = value; }
        }

        /// <summary>
        /// Specifies the user name to use for authentication to the SMTP mail server.
        /// </summary>
        [ConfigurationProperty(userName, IsRequired = true)]
        public string UserName {
            get { return (string)this[userName]; }
            set { this[userName] = value; }
        }

        /// <summary>
        /// Specifies the password to use for authentication to the SMTP mail server.
        /// </summary>
        [ConfigurationProperty(password, IsRequired = true)]
        public string Password {
            get { return (string)this[password]; }
            set { this[password] = value; }
        }
    }
}
