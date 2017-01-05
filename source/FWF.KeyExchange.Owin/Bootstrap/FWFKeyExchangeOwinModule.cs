using System;
using Autofac;

namespace FWF.KeyExchange.Owin.Bootstrap
{
    public class FWFKeyExchangeOwinModule : Module 
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RemoteIpPortEndpointIdProvider>()
                .AsSelf()
                .As<IOwinEndpointIdProvider>()
                .SingleInstance();

        }
    }
}
