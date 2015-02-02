using System.Configuration;

namespace PureLib.Configuration {
    public class PureLibConfigurationSection : ConfigurationSection {
        private const string mailConfiguration = "mailConfiguration";
        private const string cryptoConfiguration = "cryptoConfiguration";

        public const string Name = "purelibConfiguration";

        [ConfigurationProperty(mailConfiguration)]
        public MailConfiguration MailConfiguration {
            get { return (MailConfiguration)this[mailConfiguration]; }
            set { this[mailConfiguration] = value; }
        }

        [ConfigurationProperty(cryptoConfiguration)]
        public CryptoConfiguration CryptoConfiguration {
            get { return (CryptoConfiguration)this[cryptoConfiguration]; }
            set { this[cryptoConfiguration] = value; }
        }
    }
}
