using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace PureLib.Configuration {
    /// <summary>
    /// Represents a crypto element within a configuration file.
    /// </summary>
    public class CryptoConfiguration : ConfigurationElement {
        private const string ivPath = "ivpath";
        private const string keyPath = "keypath";

        /// <summary>
        /// Specifies the path of initialization vector.
        /// </summary>
        [ConfigurationProperty(ivPath, DefaultValue = @"C:\CryptoIV", IsRequired = true)]
        public string IVPath {
            get { return (string)this[ivPath]; }
            set { this[ivPath] = value; }
        }

        /// <summary>
        /// Specifies the path of secret key.
        /// </summary>
        [ConfigurationProperty(keyPath, DefaultValue = @"C:\CryptoKey", IsRequired = true)]
        public string KeyPath {
            get { return (string)this[keyPath]; }
            set { this[keyPath] = value; }
        }
    }
}
