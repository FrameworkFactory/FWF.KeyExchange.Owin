﻿using System;
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
        
        private readonly Encoding _defaultEncoding = Encoding.UTF8;

        public void Initialize(FWFKeyExchangeBootstrapper bootstrapper, IOwinContext context)
        {
            if (ReferenceEquals(bootstrapper, null))
            {
                throw new ArgumentNullException("bootstrapper");
            }
            if (ReferenceEquals(context, null))
            {
                throw new ArgumentNullException("context");
            }

            var options = bootstrapper.Resolve<KeyExchangeOptions>();

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

            // NOTE: Save the options to the OWIN context to allow child components to 
            // retrieve settings and implementation logic
            _owinContext.Environment[OwinKeyExchangeKeyNames.Bootstrapper] = bootstrapper;
        }

        public virtual Task<bool> InvokeAsync()
        {
            if (!_haveInit)
            {
                throw new InvalidOperationException("Must Initialize component before invoking");
            }

            // Retrieve KeyExchange from the IOwinContext
            var bootstrapper = _owinContext.Environment[OwinKeyExchangeKeyNames.Bootstrapper] as FWFKeyExchangeBootstrapper;

            var options = bootstrapper.Resolve<KeyExchangeOptions>();

            var keyExchangePath = options.KeyExchangePath.ToLowerInvariant();
            var keyExchangeProvider = options.KeyExchangeProvider;
            var owinEndpointIdProvider = options.EndpointIdProvider;

            var requestPath = _owinContext.Request.Path.Value.ToLowerInvariant();
            var requestMethod = _owinContext.Request.Method;

            if (requestMethod == "POST" && requestPath == keyExchangePath)
            {
                // Determine an id for the remote endpoint
                var endpointId = owinEndpointIdProvider.DetermineEndpointId(_owinContext);

                // Get key from POST data
                string keyString;
                using (var x = new StreamReader(_owinContext.Request.Body, _defaultEncoding))
                {
                    keyString = x.ReadToEnd();
                }
                var keyData = Convert.FromBase64String(keyString);

                // Give the exchange key to the key exchange provider
                keyExchangeProvider.ConfigureEndpointExchange(endpointId, keyData);

                // Write the local public key back in the response
                var publicKeyString = Convert.ToBase64String(keyExchangeProvider.PublicKey);

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
