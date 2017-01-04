using System;

namespace FWF.KeyExchange.Owin
{
    public class OwinKeyExchangeOptions
    {

        public OwinKeyExchangeOptions()
        {
            this.BitLength = KeyExchangeBitLength.Hash256;
            this.KeyExchangePath = "/keyexchange";
        }

        public IKeyExchangeProvider KeyExchangeProvider
        {
            get;
            set;
        }

        public IOwinEndpointIdProvider EndpointIdProvider
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

    }
}
