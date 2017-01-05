using System;
using Autofac;
using NUnit.Framework;
using FWF.KeyExchange.Owin.Logging;

namespace FWF.KeyExchange.Owin.Test
{
    [TestFixture]
    public class KeyExchangeProviderTest
    {

        private IKeyExchangeProvider _localKeyExchangeProvider;
        private IKeyExchangeProvider _remoteKeyExchangeProvider;


        [SetUp]
        public void Setup()
        {
            var container = TestApplicationState.Container;

            var cache = container.Resolve<ICache>();
            var logFactory = container.Resolve<ILogFactory>();

            // Get two instances to exchange keys between them
            _localKeyExchangeProvider = new KeyExchangeProvider(cache, logFactory);
            _remoteKeyExchangeProvider = new KeyExchangeProvider(cache, logFactory);
        }

        [TearDown]
        public void Teardown()
        {

        }

        [Test]
        public void Connect()
        {
            Assert.IsNotNull(_localKeyExchangeProvider.PublicKey);
            Assert.IsNotNull(_remoteKeyExchangeProvider.PublicKey);

            // 
            var localEndpointId = "ABC";
            var remoteEndpointId = "DEF";

            // NOTE: Give each other the public key
            _localKeyExchangeProvider.ConfigureEndpointExchange(remoteEndpointId, _remoteKeyExchangeProvider.PublicKey);
            _remoteKeyExchangeProvider.ConfigureEndpointExchange(localEndpointId, _localKeyExchangeProvider.PublicKey);

            // 
            Assert.IsNotNull(_localKeyExchangeProvider.IsEndpointConfigured(remoteEndpointId));
            Assert.IsNotNull(_remoteKeyExchangeProvider.IsEndpointConfigured(localEndpointId));

            // 
            var localSharedKey = _localKeyExchangeProvider.GetEndpointSharedKey(remoteEndpointId);
            var remoteSharedKey = _remoteKeyExchangeProvider.GetEndpointSharedKey(localEndpointId);

            Assert.IsNotNull(localSharedKey);
            Assert.IsNotNull(remoteSharedKey);

            //
            Assert.IsTrue(localSharedKey.IsEqualByte(remoteSharedKey));

        }

    }
}
