using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

namespace FWF.KeyExchange.Owin
{
    public class OwinKeyExchangeMessageMiddleware : OwinMiddleware
    {

        private FWFKeyExchangeBootstrapper _boostrapper;

        public OwinKeyExchangeMessageMiddleware(
            OwinMiddleware next,
            IAppBuilder appBuilder,
            FWFKeyExchangeBootstrapper boostrapper
            ) : base(next)
        {
            if (ReferenceEquals(boostrapper, null))
            {
                throw new ArgumentNullException("boostrapper");
            }

            _boostrapper = boostrapper;
        }

        public override async Task Invoke(IOwinContext context)
        {
            var handler = new OwinKeyExchangeMessageHandler();

            handler.Initialize(context);

            if (!await handler.InvokeAsync())
                await this.Next.Invoke(context);

            await handler.TeardownAsync();
        }

    }
}
