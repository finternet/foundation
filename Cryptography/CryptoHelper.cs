using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FI.Foundation.Cryptography
{
    /// <summary>
    /// Provides handy methods to Encrypt/Decrypt data.
    /// 
    /// <example>
    /// var encryptedData = CryptoHelper.EncryptAES("This is text to encrypt");
    /// var originalData = CryptoHelper.DecryptAED(encryptedData);
    /// </example>
    /// </summary>
    public static class CryptoHelper
    {
        // Stores the generated key and iv from below private key. You need to use different private key for each project, or always provide key/iv to the function

        /// <summary>
        /// Generated AES Key
        /// </summary>
        private static byte[] _AESKey = { };
        /// <summary>
        /// Generated AES IV
        /// </summary>
        private static byte[] _AESIV = { };

        /// <summary>
        /// Private key or password which will be used to encrypt data
        /// </summary>
        private const string _AESPrivateKey = "**1^H;j=!:j.u=+g9_6C5i;lP28QVs=g";
       
        /// <summary>
        /// Iteration count to generate key/iv. It is important to have same settings, if you want to deal with other systems or programming languages
        /// </summary>
        private const int ITERATION_COUNT = 1024;
        
        /// <summary>
        /// Use this in debug mode to check the IV generated. Useful in case you want to compare the values with other programming languages generated value
        /// </summary>
        /// <param name="iv">Generated IV based on predefine key</param>
        [Conditional("DEBUG")]
        public static void GetIV(ref byte[] iv)
        {
            iv = _AESIV;
        }

        /// <summary>
        /// Use this in debug mode to check the key generated. Useful in case you want to compare the values with other programming languages generated value
        /// </summary>
        /// <param name="_key">Generated key based on predefine key</param>
        [Conditional("DEBUG")]
        public static void GetKey(ref byte[] _key)
        {
            _key = _AESKey;
        }
        
        /// <summary>
        /// Encrypts string by using the passed KEY/IV. If no key has been passed, system will use the pre-defined ones. The result will be BASE64 version of encrypted data
        /// </summary>
        /// <param name="plainText">Text to be encrypted</param>
        /// <param name="key">Key to be used. Pass null to use default key. The key should be BASE64 string of actual key</param>
        /// <param name="iv">IV to be used. if key is null, it will use the default key. The iv should be BASE64 string of actual iv</param>
        /// <returns>BASE64 version of encrypted data or NULL if any error happens</returns>
        public static string EncryptAES(string plainText, string key = null, string iv = null)
        {
            try
            {
                using (var aesAlg = new AesCryptoServiceProvider())
                {
                    // Initialize Algorithm. If you want to consume encrypted data in JS or other programming languages, make sure to pass same settings. Crypto.js supports all below options.
                    aesAlg.BlockSize = 128;
                    aesAlg.KeySize = 256;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7; 


                    if (string.IsNullOrEmpty(key))
                    {
                        // If key is not generated, then generate it
                        if (_AESKey.Length == 0)
                        {
                            // get bytes from the key first
                            byte[] keyBytes = Encoding.ASCII.GetBytes(_AESPrivateKey);

                            // generate pseudo-random key
                            Rfc2898DeriveBytes derivedKey = new Rfc2898DeriveBytes(keyBytes, SHA1Managed.Create().ComputeHash(keyBytes), ITERATION_COUNT);
                            // Generate key
                            _AESKey = derivedKey.GetBytes(aesAlg.KeySize / 8);
                            // Generate IV
                            _AESIV = derivedKey.GetBytes(aesAlg.BlockSize / 8);
                        }

                        aesAlg.Key = _AESKey;
                        aesAlg.IV = _AESIV;
                    }
                    else
                    {
                        aesAlg.Key = Convert.FromBase64String(key);
                        aesAlg.IV = Convert.FromBase64String(iv);
                    }

                    using (var aes = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                    {
                        byte[] encryptedText = null;
                        using (MemoryStream msEncrypt = new MemoryStream())
                        {
                            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aes, CryptoStreamMode.Write))
                            {
                                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                                {
                                    swEncrypt.Write(plainText);
                                }
                            }
                            encryptedText = msEncrypt.ToArray();
                            return Convert.ToBase64String(encryptedText);
                        }
                    }

                }
            }
            catch (Exception er)
            {
                // log error with your favorite logging method. What about Microsoft Enterprise library?
            }
            return null;
        }

        /// <summary>
        /// Decrypts the encrypted data using the passed KEY/IV. If no key has been passed, system will use the pre-defined ones. 
        /// </summary>
        /// <param name="encryptedText">BASE64 of encrypted data</param>
        /// <param name="key">Key to be used. Pass null to use default key. The key should be BASE64 string of actual key</param>
        /// <param name="iv">IV to be used. if key is null, it will use the default key. The iv should be BASE64 string of actual iv</param>
        /// <returns>Returns the decrypted data or NULL in case of error</returns>
        public static string DecryptAES(string encryptedText, string key = null, string iv = null)
        {
            try
            {
                using (var aesAlg = new AesCryptoServiceProvider())
                {
                    // Initialize Algorithm. If data comes from JS or other programming languages, make sure to pass same settings. Crypto.js supports all below options.
                    aesAlg.BlockSize = 128;
                    aesAlg.KeySize = 256;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    if (string.IsNullOrEmpty(key))
                    {
                        // If key is not generated, then generate it
                        if (_AESKey.Length == 0)
                        {
                            // get bytes from the key first
                            byte[] keyBytes = Encoding.ASCII.GetBytes(_AESPrivateKey);

                            // generate pseudo-random key
                            Rfc2898DeriveBytes derivedKey = new Rfc2898DeriveBytes(keyBytes, SHA1Managed.Create().ComputeHash(keyBytes), ITERATION_COUNT);
                            // Generate key
                            _AESKey = derivedKey.GetBytes(aesAlg.KeySize / 8);
                            // Generate IV
                            _AESIV = derivedKey.GetBytes(aesAlg.BlockSize / 8);
                        }

                        aesAlg.Key = _AESKey;
                        aesAlg.IV = _AESIV;
                    }
                    else
                    {
                        aesAlg.Key = Convert.FromBase64String(key);
                        aesAlg.IV = Convert.FromBase64String(iv);
                    }
                    using (var aes = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                    {
                        using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
                        {
                            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aes, CryptoStreamMode.Read))
                            {
                                using (StreamReader swDecrypt = new StreamReader(csDecrypt))
                                {
                                    return swDecrypt.ReadToEnd();
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception er)
            {
                // log error with your favorite logging method. What about Microsoft Enterprise library?
            }
            return null;
        }
    }
}
