using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace FWF.KeyExchange.Owin
{
    internal class OwinKeyExchangeMessageHandler
    {

        private IOwinContext _owinContext;
        private bool _haveInit;

        private readonly Encoding _defaultEncoding = Encoding.UTF8;

        public void Initialize(IOwinContext context)
        {
            if (ReferenceEquals(context, null))
            {
                throw new ArgumentNullException("context");
            }
            
            _haveInit = true;
            _owinContext = context;
        }


        public virtual Task<bool> InvokeAsync()
        {
            // Retrieve KeyExchange from the IOwinContext
            var bootstrapper = _owinContext.Environment[OwinKeyExchangeKeyNames.Bootstrapper] as FWFKeyExchangeBootstrapper;

            var options = bootstrapper.Resolve<KeyExchangeOptions>();

            var keyExchangePath = options.KeyExchangePath.ToLowerInvariant();
            var keyExchangeProvider = options.KeyExchangeProvider;
            var owinEndpointIdProvider = options.EndpointIdProvider;
            var symmetricEncryptionProvider = options.SymmetricEncryptionProvider;

            var requestPath = _owinContext.Request.Path.Value.ToLowerInvariant();
            var requestMethod = _owinContext.Request.Method;

            if (requestMethod == "POST" && requestPath.ToLowerInvariant() == options.SendPayloadPath.ToLowerInvariant())
            {
                // Get encrypted message from POST data
                var ivString = _owinContext.Request.Query["iv"];

                if (ivString.IsMissing())
                {
                    _owinContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    _owinContext.Response.Headers.Set("Content-Type", "text/plain");
                    _owinContext.Response.ContentType = "text/plain";

                    _owinContext.Response.Write("Must supply the iv query parameter");

                    return Task.FromResult(true);
                }

                //
                var iv = Convert.FromBase64String(ivString);

                // NOTE: Not the best implementation to receive the entire request and parse
                // as a string...
                byte[] messageData;
                using (var stream = new MemoryStream())
                {
                    _owinContext.Request.Body.CopyTo(stream);

                    messageData = stream.ToArray();
                }

                //
                var endpointId = owinEndpointIdProvider.DetermineEndpointId(_owinContext);

                // 
                var sharedKey = keyExchangeProvider.GetEndpointSharedKey(endpointId);

                // Decrypt the message
                var decryptedData = symmetricEncryptionProvider.Decrypt(
                    sharedKey,
                    iv,
                    messageData
                    );

                _owinContext.Response.StatusCode = (int)HttpStatusCode.OK;

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

