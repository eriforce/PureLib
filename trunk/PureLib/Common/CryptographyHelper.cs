using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PureLib.Common {
    /// <summary>
    /// Provides encryption and decryption methods.
    /// </summary>
    public static class CryptographyHelper {
        /// <summary>
        /// Computes the hash value of a file.
        /// </summary>
        /// <typeparam name="T">T is a HashAlgorithm</typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] CreateFileHash<T>(this string path) where T : HashAlgorithm {
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using (HashAlgorithm hashAlgorithm = Utility.GetInstance<T>()) {
                    return hashAlgorithm.ComputeHash(stream);
                }
            }
        }

        /// <summary>
        /// Computes the hash value of a string.
        /// </summary>
        /// <typeparam name="T">T is a HashAlgorithm</typeparam>
        /// <param name="plaintext"></param>
        /// <returns></returns>
        public static byte[] CreateHash<T>(this string plaintext) where T : HashAlgorithm {
            return CreateHash<T>(GetBytes(plaintext));
        }

        /// <summary>
        /// Computes the hash value of binary data.
        /// </summary>
        /// <typeparam name="T">T is a HashAlgorithm</typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] CreateHash<T>(this byte[] bytes) where T : HashAlgorithm {
            using (HashAlgorithm hashAlgorithm = Utility.GetInstance<T>()) {
                return CreateHash(bytes, hashAlgorithm);
            }
        }

        /// <summary>
        /// Computes the HMAC value of binary data.
        /// </summary>
        /// <typeparam name="T">T is a HMAC</typeparam>
        /// <param name="plaintext"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] CreateHMAC<T>(this string plaintext, byte[] key) where T : HMAC {
            using (HMAC hmac = Utility.GetInstance<T>(key)) {
                return CreateHash(GetBytes(plaintext), hmac);
            }
        }

        /// <summary>
        /// Computes the HMAC value of binary data.
        /// </summary>
        /// <typeparam name="T">T is a HMAC</typeparam>
        /// <param name="bytes"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] CreateHMAC<T>(this byte[] bytes, byte[] key) where T : HMAC {
            using (HMAC hmac = Utility.GetInstance<T>(key)) {
                return CreateHash(bytes, hmac);
            }
        }

        /// <summary>
        /// Encryptos binary data.
        /// </summary>
        /// <typeparam name="T">T is a SymmetricAlgorithm</typeparam>
        /// <param name="plaintext"></param>
        /// <param name="iv">The initialization vector of SymmetricAlgorithm</param>
        /// <param name="key">The secret key of SymmetricAlgorithm</param>
        /// <returns></returns>
        public static byte[] Encrypto<T>(this byte[] plaintext, byte[] iv, byte[] key) where T : SymmetricAlgorithm {
            using (SymmetricAlgorithm symmetricAlgorithm = Utility.GetInstance<T>()) {
                symmetricAlgorithm.IV = iv;
                symmetricAlgorithm.Key = key;
                return symmetricAlgorithm.CreateEncryptor().TransformFinalBlock(plaintext, 0, plaintext.Length);
            }
        }

        /// <summary>
        /// Decryptos binary data.
        /// </summary>
        /// <typeparam name="T">T is a SymmetricAlgorithm</typeparam>
        /// <param name="ciphertext"></param>
        /// <param name="iv">The initialization vector of SymmetricAlgorithm</param>
        /// <param name="key">The secret key of SymmetricAlgorithm</param>
        /// <returns></returns>
        public static byte[] Decrypto<T>(this byte[] ciphertext, byte[] iv, byte[] key) where T : SymmetricAlgorithm {
            using (SymmetricAlgorithm symmetricAlgorithm = Utility.GetInstance<T>()) {
                symmetricAlgorithm.IV = iv;
                symmetricAlgorithm.Key = key;
                return symmetricAlgorithm.CreateDecryptor().TransformFinalBlock(ciphertext, 0, ciphertext.Length);
            }
        }

        /// <summary>
        /// Generates initialization vector and secret key for a SymmetricAlgorithm.
        /// </summary>
        /// <typeparam name="T">T is a SymmetricAlgorithm</typeparam>
        /// <param name="iv">The initialization vector of SymmetricAlgorithm</param>
        /// <param name="key">The secret key of SymmetricAlgorithm</param>
        /// <returns></returns>
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

        private static byte[] GetBytes(string text) {
            return Encoding.UTF8.GetBytes(text);
        }
    }
}
