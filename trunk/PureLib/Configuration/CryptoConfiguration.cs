using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using PureLib.Common;

namespace PureLib.Configuration {
    public class CryptoConfiguration : ConfigurationElement {
        private const string iv = "iv";
        private const string key = "key";

        public byte[] IV {
            get {
                return IvString.IsNullOrEmpty() ?
                    null : IvString.FromBase64String();
            }
        }
        public byte[] Key {
            get {
                return KeyString.IsNullOrEmpty() ?
                    null : KeyString.FromBase64String();
            }
        }

        [ConfigurationProperty(iv, IsRequired = true)]
        public string IvString {
            get { return (string)this[iv]; }
        }

        [ConfigurationProperty(key, IsRequired = true)]
        public string KeyString {
            get { return (string)this[key]; }
        }
    }
}
