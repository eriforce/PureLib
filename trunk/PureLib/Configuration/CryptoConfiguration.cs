using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using PureLib.Common;

namespace PureLib.Configuration {
    public class CryptoConfiguration : ConfigurationElement {
        private const string iv = "aesIv";
        private const string key = "aesKey";
        private const string thumbprint = "thumbprint";
        private const string storeName = "storeName";
        private const string storeLocation = "storeLocation";

        private X509Certificate2 _certificate;

        public byte[] DecryptedIv {
            get { return DecryptFromString(IvString); }
        }
        public byte[] DecryptedKey {
            get { return DecryptFromString(KeyString); }
        }
        public X509Certificate2 Certificate {
            get {
                if (Thumbprint.IsNullOrEmpty())
                    return null;

                if (_certificate == null)
                    _certificate = CryptographyHelper.GetCertificate(StoreName, StoreLocation, X509FindType.FindByThumbprint, Thumbprint);
                return _certificate;
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

        [ConfigurationProperty(thumbprint)]
        public string Thumbprint {
            get { return (string)this[thumbprint]; }
        }

        [ConfigurationProperty(storeName, DefaultValue = StoreName.My)]
        public StoreName StoreName {
            get { return (StoreName)this[storeName]; }
        }

        [ConfigurationProperty(storeLocation, DefaultValue = StoreLocation.LocalMachine)]
        public StoreLocation StoreLocation {
            get { return (StoreLocation)this[storeLocation]; }
        }

        private byte[] DecryptFromString(string s) {
            if (s.IsNullOrEmpty())
                return null;

            byte[] bin = s.FromBase64String();
            if (Certificate != null)
                bin = bin.Decrypt((RSACryptoServiceProvider)Certificate.PrivateKey);
            return bin;
        }
    }
}
