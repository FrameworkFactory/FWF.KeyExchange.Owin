using System;

namespace FWF.KeyExchange.Owin
{
    public interface ISymmetricEncryptionProvider
    {

        byte[] Encrypt(byte[] key, byte[] plainTextData, out byte[] iv);


        byte[] Decrypt(byte[] key, byte[] iv, byte[] encryptedData);

    }
}
