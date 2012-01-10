using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace PureLib.Configuration {
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

        [ConfigurationProperty(from, IsRequired = true)]
        public string From {
            get { return (string)this[from]; }
            set { this[from] = value; }
        }

        [ConfigurationProperty(to, IsRequired = true)]
        public string To {
            get { return (string)this[to]; }
            set { this[to] = value; }
        }

        [ConfigurationProperty(cc)]
        public string CC {
            get { return (string)this[cc]; }
            set { this[cc] = value; }
        }

        [ConfigurationProperty(bcc)]
        public string Bcc {
            get { return (string)this[bcc]; }
            set { this[bcc] = value; }
        }

        [ConfigurationProperty(host, IsRequired = true)]
        public string Host {
            get { return (string)this[host]; }
            set { this[host] = value; }
        }

        [ConfigurationProperty(port, DefaultValue = 25, IsRequired = true)]
        public int Port {
            get { return (int)this[port]; }
            set { this[port] = value; }
        }

        [ConfigurationProperty(ssl, DefaultValue = false, IsRequired = true)]
        public bool EnableSsl {
            get { return (bool)this[ssl]; }
            set { this[ssl] = value; }
        }

        [ConfigurationProperty(userName, IsRequired = true)]
        public string UserName {
            get { return (string)this[userName]; }
            set { this[userName] = value; }
        }

        [ConfigurationProperty(password, IsRequired = true)]
        public string Password {
            get { return (string)this[password]; }
            set { this[password] = value; }
        }
    }
}
