using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Autofac;
using FWF.KeyExchange.Bootstrap;

namespace FWF.KeyExchange.Sample.OwinApiClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var rootUrl = "https://localhost:61000/";


            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<FWFKeyExchangeModule>();

            var container = containerBuilder.Build();

            var keyExchangeProvider = container.Resolve<IKeyExchangeProvider>();
            var random = container.Resolve<IRandom>();

            keyExchangeProvider.Start();
            
            using (var http = new HttpClient())
            {
                // NOTE: GET the root url to make sure the endpoint is there...
                using (var httpResponse = http.GetAsync(rootUrl).Result)
                {
                }

                // Give the public key to exchange the key with the remote api
                var exchangeUrl = rootUrl + "exchange";
                var postData = new StringContent(Convert.ToBase64String(keyExchangeProvider.PublicKey));

                using (var httpResponse = http.PostAsync(exchangeUrl, postData).Result)
                {
                    var remotePublicKey = httpResponse.Content.ReadAsStringAsync().Result;

                    var remotePublicKeyData = Convert.FromBase64String(remotePublicKey);

                    keyExchangeProvider.Exchange(remotePublicKeyData);
                }

                // Create a message
                var plainTextMessage = random.AnyString(1024);
                byte[] iv;
                byte[] encryptedMessage;

                // Encrypt the message with the shared key and a new IV
                using (Aes aes = new AesCryptoServiceProvider())
                {
                    aes.Key = keyExchangeProvider.SharedKey;
                    iv = aes.IV;

                    using (var stream = new MemoryStream())
                    using (var cs = new CryptoStream(stream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] plainTextData = Encoding.UTF8.GetBytes(plainTextMessage);
                        cs.Write(plainTextData, 0, plainTextData.Length);
                        cs.Close();
                        encryptedMessage = stream.ToArray();
                    }
                }

                var ivString = Convert.ToBase64String(iv);
                var encodedIv = HttpUtility.UrlEncode(ivString);
                
                // Send the encrypted message
                var sendUrl = rootUrl + "send?iv=" + encodedIv;
                var sendData = new ByteArrayContent(encryptedMessage);

                string decryptedMessageFromRemoteEndpoint;

                using (var httpResponse = http.PostAsync(sendUrl, sendData).Result)
                {
                    decryptedMessageFromRemoteEndpoint = httpResponse.Content.ReadAsStringAsync().Result;
                }

                // NOTE: For this test, the response should be identical to the plain text message
                var isMatch = plainTextMessage.Equals(decryptedMessageFromRemoteEndpoint);

                if (isMatch)
                {
                    Console.WriteLine("Success!");
                }
                else
                {
                    Console.WriteLine("Failure! - Message response from remote endpoint did not match");
                }
            }


        }
    }
}
