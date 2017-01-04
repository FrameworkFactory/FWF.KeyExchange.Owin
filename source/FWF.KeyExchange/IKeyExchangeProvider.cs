using System;

namespace FWF.KeyExchange
{
    public interface IKeyExchangeProvider : IRunnable 
    {

        KeyExchangeBitLength BitLength
        {
            get; set;
        }

        byte[] PublicKey
        {
            get;
        }

        void ConfigureEndpointExchange(string endpointId, byte[] remotePublicKey);

        bool IsEndpointConfigured(string endpointId);

        byte[] SharedKey
        {
            get;
        }
        
    }
}
