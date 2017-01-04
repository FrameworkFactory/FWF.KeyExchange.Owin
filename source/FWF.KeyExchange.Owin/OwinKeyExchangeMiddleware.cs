using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

namespace FWF.KeyExchange.Owin
{
    public class OwinKeyExchangeMiddleware : OwinMiddleware
    {

        public OwinKeyExchangeOptions Options { get; set; }

        public OwinKeyExchangeMiddleware(
            OwinMiddleware next,
            IAppBuilder appBuilder,
            OwinKeyExchangeOptions options
            ): base(next)
        {
            if (ReferenceEquals(options, null))
            {
                throw new ArgumentNullException("options");
            }

            this.Options = options;
        }

        public override async Task Invoke(IOwinContext context)
        {
            var handler = new OwinKeyExchangeHandler();

            handler.Initialize(this.Options, context);

            if (!await handler.InvokeAsync())
                await this.Next.Invoke(context);

            await handler.TeardownAsync();
        }
        
    }
}
