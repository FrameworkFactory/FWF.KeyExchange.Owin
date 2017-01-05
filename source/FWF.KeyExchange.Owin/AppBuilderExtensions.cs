using System;
using Owin;

namespace FWF.KeyExchange.Owin
{
    public static class AppBuilderExtensions
    {

        public static void UseKeyExchange(
            this IAppBuilder appBuilder, 
            FWFKeyExchangeBootstrapper bootstrapper
            )
        {
            appBuilder.Use(typeof(OwinKeyExchangeMiddleware), new object[]
            {
                appBuilder,
                bootstrapper
            });
        }

        public static void UseKeyExchangeMessage(
            this IAppBuilder appBuilder,
            FWFKeyExchangeBootstrapper bootstrapper
            )
        {
            appBuilder.Use(typeof(OwinKeyExchangeMessageMiddleware), new object[]
            {
                appBuilder,
                bootstrapper
            });
        }

    }
}
