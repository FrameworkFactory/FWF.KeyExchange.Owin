using System;
using System.IO;
using FWF.KeyExchange.Logging;

namespace FWF.KeyExchange
{
    internal class KeyExchangeProvider : Startable, IKeyExchangeProvider
    {

        private byte[] _publicKey;
        private byte[] _sharedKey;

        private readonly int PublicKeyByteLength = 256;

        private readonly IRandom _random;
        private readonly ILog _log;

        public KeyExchangeProvider(
            IRandom random,
            ILogFactory logFactory
            )
        {
            _random = random;

            _log = logFactory.CreateForType(this);
        }

        protected override void OnStart()
        {
            _publicKey = _random.AnyBytes(PublicKeyByteLength);
        }

        protected override void OnStop()
        {
            _publicKey = null;
            _sharedKey = null;
        }


        public byte[] PublicKey
        {
            get
            {
                return _publicKey;
            }
        }

        public byte[] SharedKey
        {
            get
            {
                return _sharedKey;
            }
        }

        public void Exchange(byte[] remotePublicKey)
        {
            //BigInteger.ModPow();


        }

    }
}
