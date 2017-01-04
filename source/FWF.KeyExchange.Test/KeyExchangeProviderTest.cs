using System;
using Autofac;
using NUnit.Framework;

namespace FWF.KeyExchange.Test
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

            // Get two instances to exchange keys between them
            _localKeyExchangeProvider = container.Resolve<IKeyExchangeProvider>();
            _remoteKeyExchangeProvider = container.Resolve<IKeyExchangeProvider>();
        }

        [TearDown]
        public void Teardown()
        {

        }

        [Test]
        public void Connect()
        {
            _localKeyExchangeProvider.Start();
            _remoteKeyExchangeProvider.Start();

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
            Assert.IsNotNull(_localKeyExchangeProvider.SharedKey);
            Assert.IsNotNull(_remoteKeyExchangeProvider.SharedKey);

            //
            Assert.IsTrue(_localKeyExchangeProvider.SharedKey.IsEqualByte(_remoteKeyExchangeProvider.SharedKey));

        }

    }
}
