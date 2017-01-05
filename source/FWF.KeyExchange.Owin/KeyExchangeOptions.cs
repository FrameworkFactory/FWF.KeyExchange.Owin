using System;

namespace FWF.KeyExchange.Owin
{
    public class KeyExchangeOptions
    {

        public KeyExchangeOptions()
        {
            this.BitLength = KeyExchangeBitLength.Hash256;
            this.KeyExchangePath = "/keyexchange";
            this.SendPayloadPath = "/sendpayload";
        }
        
        public IKeyExchangeProvider KeyExchangeProvider
        {
            get;
            set;
        }

        public IEndpointIdProvider EndpointIdProvider
        {
            get;
            set;
        }

        public ISymmetricEncryptionProvider SymmetricEncryptionProvider
        {
            get;
            set;
        }

        public KeyExchangeBitLength BitLength
        {
            get;
            set;
        }

        public string KeyExchangePath
        {
            get;
            set;
        }

        public string SendPayloadPath
        {
            get;
            set;
        }

    }
}
