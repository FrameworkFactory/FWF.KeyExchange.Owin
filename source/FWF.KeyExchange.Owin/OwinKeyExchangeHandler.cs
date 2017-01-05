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
        
        private IOwinContext _owinContext;
        private bool _haveInit;

        private IKeyExchangeProvider _keyExchangeProvider;
        private IOwinEndpointIdProvider _owinEndpointIdProvider;
        private string _keyExchangePath;

        private readonly Encoding _defaultEncoding = Encoding.UTF8;

        public void Initialize(OwinKeyExchangeOptions options, IOwinContext context)
        {
            if (ReferenceEquals(options, null))
            {
                throw new ArgumentNullException("options");
            }
            if (ReferenceEquals(context, null))
            {
                throw new ArgumentNullException("context");
            }
            if (ReferenceEquals(options.KeyExchangeProvider, null))
            {
                throw new ArgumentException("Must supply a KeyExchangeProvider");
            }
            if (ReferenceEquals(options.EndpointIdProvider, null))
            {
                throw new ArgumentException("Must supply a EndpointIdProvider");
            }
            if (options.KeyExchangePath.IsMissing())
            {
                throw new ArgumentException("Must supply a value KeyExchangePath");
            }

            _haveInit = true;

            _owinContext = context;

            //
            _keyExchangePath = options.KeyExchangePath.ToLowerInvariant();
            _keyExchangeProvider = options.KeyExchangeProvider;
            _owinEndpointIdProvider = options.EndpointIdProvider;

            // NOTE: Save the AutoFac IoC container to the OWIN context to allow
            // child components to retrieve implementations from the container
            _owinContext.Environment[OwinKeyExchangeEnvironment.KeyExchangeProvider] = options.KeyExchangeProvider;
            _owinContext.Environment[OwinKeyExchangeEnvironment.EndpointIdProvider] = options.EndpointIdProvider;
            _owinContext.Environment[OwinKeyExchangeEnvironment.SymmetricEncryptionProvider] = options.SymmetricEncryptionProvider;
        }

        public virtual Task<bool> InvokeAsync()
        {
            if (!_haveInit)
            {
                throw new InvalidOperationException("Must Initialize component before invoking");
            }

            var requestPath = _owinContext.Request.Path.Value.ToLowerInvariant();
            var requestMethod = _owinContext.Request.Method;

            if (requestMethod == "POST" && requestPath == _keyExchangePath)
            {
                // Determine an id for the remote endpoint
                var endpointId = _owinEndpointIdProvider.DetermineEndpointId(_owinContext);

                // Get key from POST data
                string keyString;
                using (var x = new StreamReader(_owinContext.Request.Body, _defaultEncoding))
                {
                    keyString = x.ReadToEnd();
                }
                var keyData = Convert.FromBase64String(keyString);

                // Give the exchange key to the key exchange provider
                _keyExchangeProvider.ConfigureEndpointExchange(endpointId, keyData);

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
