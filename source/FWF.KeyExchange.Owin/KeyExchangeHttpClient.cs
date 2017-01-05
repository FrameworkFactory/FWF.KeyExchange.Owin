using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Web;

namespace FWF.KeyExchange.Owin
{
    public class KeyExchangeHttpClient : HttpClient
    {

        private readonly FWFKeyExchangeBootstrapper _bootstrapper;

        private readonly KeyExchangeOptions _options;
        private readonly IKeyExchangeProvider _keyExchangeProvider;
        private readonly ISymmetricEncryptionProvider _symmetricEncryptionProvider;
        private readonly IEndpointIdProvider _endpointIdProvider;
        
        private readonly IDictionary<string, bool> _configuredEndpoints = new Dictionary<string, bool>();

        public KeyExchangeHttpClient(FWFKeyExchangeBootstrapper bootstrapper)
        {
            _bootstrapper = bootstrapper;

            _options = bootstrapper.Resolve<KeyExchangeOptions>();
            _keyExchangeProvider = _bootstrapper.Resolve<IKeyExchangeProvider>();
            _symmetricEncryptionProvider = _bootstrapper.Resolve<ISymmetricEncryptionProvider>();
            _endpointIdProvider = _bootstrapper.Resolve<IEndpointIdProvider>();
        }

        public override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
            )
        {
            // NOTE: Determine the endpoint we are sending a request to and
            // see if we have exchanged keys yet...
            SetupExchange(request.RequestUri);

            return base.SendAsync(request, cancellationToken);
        }

        public Task SetupExchange(Uri remoteHost)
        {
            // NOTE: Determine the endpoint we are sending a request to and
            // see if we have exchanged keys yet...

            var endpointId = _endpointIdProvider.DetermineEndpointId(remoteHost);

            if (_configuredEndpoints.ContainsKey(endpointId))
            {
                return Task.CompletedTask;
            }

            // Before sending the request overridden request - send another to exchange keys
            using (var http = new HttpClient())
            {
                // Give the public key to exchange the key with the remote api
                var publicKeyString = Convert.ToBase64String(_keyExchangeProvider.PublicKey);

                var rootUrl = remoteHost.GetLeftPart(UriPartial.Authority);

                var exchangeUrl = rootUrl + _options.KeyExchangePath;

                var postData = new StringContent(publicKeyString);

                using (var httpResponse = http.PostAsync(exchangeUrl, postData).Result)
                {
                    var remotePublicKey = httpResponse.Content.ReadAsStringAsync().Result;

                    var remotePublicKeyData = Convert.FromBase64String(remotePublicKey);

                    _keyExchangeProvider.ConfigureEndpointExchange(endpointId, remotePublicKeyData);
                }
            }

            return Task.CompletedTask;
        }

        public Task SendPayload(Uri remoteHost, string payloadString)
        {
            return SendPayload(
                remoteHost,
                new MemoryStream(Encoding.UTF8.GetBytes(payloadString))
                );
        }

        public Task SendPayload(Uri remoteHost, Stream payloadStream)
        {
            //
            var endpointId = _endpointIdProvider.DetermineEndpointId(remoteHost);

            if (!_configuredEndpoints.ContainsKey(endpointId))
            {
                SetupExchange(remoteHost);
            }

            //
            var rootUrl = remoteHost.GetLeftPart(UriPartial.Authority);

            // Get the shared key for the remote endpoint
            var sharedKey = _keyExchangeProvider.GetEndpointSharedKey(endpointId);
            byte[] iv;

            // TMP: Temporary stream logic until the encyption routines are updated...
            byte[] plainTextData;
            using (var memStream = new MemoryStream())
            {
                payloadStream.CopyTo(memStream);

                plainTextData = memStream.ToArray();
            }

            // Encrypt the message with the shared key and a new IV
            var encryptedMessage = _symmetricEncryptionProvider.Encrypt(
                sharedKey,
                plainTextData,
                out iv
                );

            var ivString = Convert.ToBase64String(iv);
            var encodedIv = HttpUtility.UrlEncode(ivString);

            // Send the encrypted message
            var sendUrl = rootUrl + _options.SendPayloadPath + "?iv=" + encodedIv;
            var sendData = new ByteArrayContent(encryptedMessage);

            using (var httpResponse = this.PostAsync(sendUrl, sendData).Result)
            {
            }

            return Task.CompletedTask;
        }



    }
}
