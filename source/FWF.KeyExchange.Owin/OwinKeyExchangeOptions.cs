using System;
using Autofac;
using FWF.KeyExchange.Bootstrap;
using FWF.KeyExchange.Owin.Bootstrap;

namespace FWF.KeyExchange.Owin
{
    public class OwinKeyExchangeOptions
    {

        public OwinKeyExchangeOptions()
        {
            this.BitLength = KeyExchangeBitLength.Hash256;
            this.KeyExchangePath = "/keyexchange";

            // NOTE: The IoC container strategy could be (much) different than the
            // implementation that these components will be a part of.  Using this
            // Options component to build and contain the AutoFac IoC container
            // seems to be the best approach to abstract the complexities of IoC, but
            // still allow different implementations to be set at the property level.
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<FWFKeyExchangeModule>();
            containerBuilder.RegisterModule<FWFKeyExchangeOwinModule>();
            var container = containerBuilder.Build();
            
            this.KeyExchangeProvider = container.Resolve<IKeyExchangeProvider>();
            this.EndpointIdProvider = container.Resolve<IOwinEndpointIdProvider>();
            this.SymmetricEncryptionProvider = container.Resolve<ISymmetricEncryptionProvider>();
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

    }
}
