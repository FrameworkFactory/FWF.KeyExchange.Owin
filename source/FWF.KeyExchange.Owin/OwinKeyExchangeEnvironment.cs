using System;

namespace FWF.KeyExchange.Owin
{
    public static class OwinKeyExchangeEnvironment
    {
        public static string KeyExchangeProvider = "fwf.KeyExchange.KeyExchangeProvider";
        public static string EndpointIdProvider = "fwf.KeyExchange.EndpointIdProvider";
        public static string SymmetricEncryptionProvider = "fwf.KeyExchange.SymmetricEncryptionProvider";
    }
}
