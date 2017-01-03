using System;

namespace FWF.KeyExchange
{
    public interface IKeyExchangeProvider : IRunnable 
    {

        byte[] PublicKey
        {
            get;
        }

        byte[] SharedKey
        {
            get;
        }

        void Exchange(byte[] remotePublicKey);
        
    }
}
