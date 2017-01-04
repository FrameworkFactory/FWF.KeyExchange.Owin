using System;
using Owin;

namespace FWF.KeyExchange.Owin
{
    public static class AppBuilderExtensions
    {

        public static void UseKeyExchange(this IAppBuilder appBuilder, OwinKeyExchangeOptions options)
        {
            appBuilder.Use(typeof(OwinKeyExchangeMiddleware), new object[]
            {
                appBuilder,
                options
            });
        }

    }
}
