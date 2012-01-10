using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace PureLib.Configuration {
    public class CryptoConfiguration : ConfigurationElement {
        private const string ivPath = "ivpath";
        private const string keyPath = "keypath";

        [ConfigurationProperty(ivPath, DefaultValue = @"C:\CryptoIV", IsRequired = true)]
        public string IVPath {
            get { return (string)this[ivPath]; }
            set { this[ivPath] = value; }
        }

        [ConfigurationProperty(keyPath, DefaultValue = @"C:\CryptoKey", IsRequired = true)]
        public string KeyPath {
            get { return (string)this[keyPath]; }
            set { this[keyPath] = value; }
        }
    }
}
