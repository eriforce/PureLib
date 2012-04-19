using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PureLib.Common {
    public static class CryptographyHelper {
        private static readonly Encoding _defaultEncoding = Encoding.UTF8;

        public static byte[] GenerateSalt(int length) {
            byte[] randomNumberBuffer = new byte[length];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider()) {
                rngCsp.GetBytes(randomNumberBuffer);
            }
            return randomNumberBuffer;
        }

        public static byte[] CreateFileHash<T>(this string path) where T : HashAlgorithm {
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (HashAlgorithm hashAlgorithm = Utility.GetInstance<T>()) {
                    return hashAlgorithm.ComputeHash(stream);
                }
            }
        }

        public static byte[] CreateHash<T>(this string plaintext) where T : HashAlgorithm {
            return CreateHash<T>(_defaultEncoding.GetBytes(plaintext));
        }

        public static byte[] CreateHash<T>(this byte[] bytes) where T : HashAlgorithm {
            using (HashAlgorithm hashAlgorithm = Utility.GetInstance<T>()) {
                return CreateHash(bytes, hashAlgorithm);
            }
        }

        public static byte[] CreateHMAC<T>(this string plaintext, byte[] key) where T : HMAC {
            using (HMAC hmac = Utility.GetInstance<T>(key)) {
                return CreateHash(_defaultEncoding.GetBytes(plaintext), hmac);
            }
        }

        public static byte[] CreateHMAC<T>(this byte[] bytes, byte[] key) where T : HMAC {
            using (HMAC hmac = Utility.GetInstance<T>(key)) {
                return CreateHash(bytes, hmac);
            }
        }

        public static byte[] Encrypto<T>(this byte[] plaintext, byte[] iv, byte[] key) where T : SymmetricAlgorithm {
            using (SymmetricAlgorithm symmetricAlgorithm = Utility.GetInstance<T>()) {
                symmetricAlgorithm.IV = iv;
                symmetricAlgorithm.Key = key;
                return symmetricAlgorithm.CreateEncryptor().TransformFinalBlock(plaintext, 0, plaintext.Length);
            }
        }

        public static byte[] Decrypto<T>(this byte[] ciphertext, byte[] iv, byte[] key) where T : SymmetricAlgorithm {
            using (SymmetricAlgorithm symmetricAlgorithm = Utility.GetInstance<T>()) {
                symmetricAlgorithm.IV = iv;
                symmetricAlgorithm.Key = key;
                return symmetricAlgorithm.CreateDecryptor().TransformFinalBlock(ciphertext, 0, ciphertext.Length);
            }
        }

        public static void GenerateIVAndKey<T>(out byte[] iv, out byte[] key) where T : SymmetricAlgorithm {
            using (SymmetricAlgorithm symmetricAlgorithm = Utility.GetInstance<T>()) {
                symmetricAlgorithm.GenerateIV();
                symmetricAlgorithm.GenerateKey();
                iv = symmetricAlgorithm.IV;
                key = symmetricAlgorithm.Key;
            }
        }

        private static byte[] CreateHash(byte[] bytes, HashAlgorithm hashAlgorithm) {
            return hashAlgorithm.ComputeHash(bytes);
        }
    }
}
