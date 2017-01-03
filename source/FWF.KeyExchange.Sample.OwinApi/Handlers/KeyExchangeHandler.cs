using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace FWF.KeyExchange.Sample.OwinApi.Handlers
{
    internal class KeyExchangeHandler
    {

        private readonly IKeyExchangeProvider _keyExchangeProvider;

        private readonly Encoding _defaultEncoding = Encoding.UTF8;

        public KeyExchangeHandler(
            IKeyExchangeProvider keyExchangeProvider
            )
        {
            _keyExchangeProvider = keyExchangeProvider;
        }
        
        public Task Handle(IOwinContext context, Func<Task> next)
        {
            var requestPath = context.Request.Path.Value.ToLowerInvariant();
            var requestMethod = context.Request.Method;

            if (requestMethod == "POST" && requestPath == "/exchange")
            {
                // Get key from POST data
                string keyString;
                using (var x = new StreamReader(context.Request.Body, _defaultEncoding))
                {
                    keyString = x.ReadToEnd();
                }
                var keyData = Convert.FromBase64String(keyString);

                // Give the exchange key to the key exchange provider
                _keyExchangeProvider.Exchange(keyData);

                // Write the local public key back in the response
                var publicKeyString = Convert.ToBase64String(_keyExchangeProvider.PublicKey);

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.Headers.Set("Content-Type", "text/plain");
                context.Response.ContentType = "text/plain";
                context.Response.Write(publicKeyString);

                return Task.CompletedTask;
            }

            return next();
        }


    }
}
