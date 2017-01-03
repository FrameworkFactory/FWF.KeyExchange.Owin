using System;
using Autofac;
using FWF.KeyExchange.Sample.OwinApi.Handlers;

namespace FWF.KeyExchange.Sample.OwinApi.Bootstrap
{
    public class FWFKeyExchangeSampleOwinApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<OwinApiService>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<KeyExchangeHandler>().AsSelf().SingleInstance();
            builder.RegisterType<MessageSendHandler>().AsSelf().SingleInstance();
        }
    }
}
