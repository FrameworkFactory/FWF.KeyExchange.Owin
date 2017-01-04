using System;

namespace FWF.KeyExchange.Owin
{
    public class OwinKeyExchangeOptions
    {

        public IKeyExchangeProvider KeyExchangeProvider
        {
            get; set;
        }
    }
}
