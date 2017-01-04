using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace FWF.KeyExchange.Owin
{
    internal class OwinKeyExchangeHandler
    {
        
        private IKeyExchangeProvider _keyExchangeProvider;
        private IOwinContext _owinContext;
        private bool _haveInit;

        private readonly Encoding _defaultEncoding = Encoding.UTF8;

        public void Initialize(OwinKeyExchangeOptions options, IOwinContext context)
        {
            _haveInit = true;

            _owinContext = context;

            _keyExchangeProvider = options.KeyExchangeProvider;
        }

        public virtual Task<bool> InvokeAsync()
        {
            if (!_haveInit)
            {
                throw new InvalidOperationException("Must Initialize component before invoking");
            }

            var requestPath = _owinContext.Request.Path.Value.ToLowerInvariant();
            var requestMethod = _owinContext.Request.Method;

            if (requestMethod == "POST" && requestPath == "/keyexchange")
            {
                // Get key from POST data
                string keyString;
                using (var x = new StreamReader(_owinContext.Request.Body, _defaultEncoding))
                {
                    keyString = x.ReadToEnd();
                }
                var keyData = Convert.FromBase64String(keyString);

                // Give the exchange key to the key exchange provider
                _keyExchangeProvider.Exchange(keyData);

                // Write the local public key back in the response
                var publicKeyString = Convert.ToBase64String(_keyExchangeProvider.PublicKey);

                _owinContext.Response.StatusCode = (int)HttpStatusCode.OK;
                _owinContext.Response.Headers.Set("Content-Type", "text/plain");
                _owinContext.Response.ContentType = "text/plain";
                _owinContext.Response.Write(publicKeyString);

                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public async Task TeardownAsync()
        {
            await Task.CompletedTask;
        }



    }
}
