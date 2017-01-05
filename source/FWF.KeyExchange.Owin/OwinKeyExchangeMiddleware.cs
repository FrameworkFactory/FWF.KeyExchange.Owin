using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

namespace FWF.KeyExchange.Owin
{
    public class OwinKeyExchangeMiddleware : OwinMiddleware
    {

        private FWFKeyExchangeBootstrapper _boostrapper;

        public OwinKeyExchangeMiddleware(
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

            var options = _boostrapper.Resolve<KeyExchangeOptions>();
            
            if (ReferenceEquals(options.KeyExchangeProvider, null))
            {
                options.KeyExchangeProvider = _boostrapper.Resolve<IKeyExchangeProvider>();
            }
            if (ReferenceEquals(options.EndpointIdProvider, null))
            {
                options.EndpointIdProvider = _boostrapper.Resolve<IEndpointIdProvider>();
            }
            if (ReferenceEquals(options.SymmetricEncryptionProvider, null))
            {
                options.SymmetricEncryptionProvider = _boostrapper.Resolve<ISymmetricEncryptionProvider>();
            }
        }

        public override async Task Invoke(IOwinContext context)
        {
            var handler = new OwinKeyExchangeHandler();

            handler.Initialize(_boostrapper, context);

            if (!await handler.InvokeAsync())
                await this.Next.Invoke(context);

            await handler.TeardownAsync();
        }
        
    }
}
