using System;
using System.IO;
using System.Security.Cryptography;

namespace FWF.KeyExchange.Owin
{
    internal class SymmetricEncryptionProvider : ISymmetricEncryptionProvider
    {


        public byte[] Encrypt(byte[] key, byte[] plainTextData, out byte[] iv)
        {
            byte[] result;
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                iv = aes.IV;

                using (var stream = new MemoryStream())
                using (var cs = new CryptoStream(stream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(plainTextData, 0, plainTextData.Length);
                    cs.Close();

                    result = stream.ToArray();
                }
            }

            return result;
        }

        public byte[] Decrypt(byte[] key, byte[] iv, byte[] encryptedData)
        {
            byte[] result;

            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var stream = new MemoryStream())
                {
                    using (var cs = new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptedData, 0, encryptedData.Length);
                        cs.Close();

                        result = stream.ToArray();
                    }
                }
            }

            return result;
        }
    }
}
