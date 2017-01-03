using System;
using System.Security.Cryptography;

namespace FWF.KeyExchange
{
    internal class DiffieHellmanFromFramework : Startable, IKeyExchangeProvider, IDisposable
    {

        private byte[] _publicKey;
        private byte[] _sharedKey;

        private readonly ECDiffieHellmanCng _cng;

        public DiffieHellmanFromFramework()
        {
            _cng = new ECDiffieHellmanCng
            {
                KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
                HashAlgorithm = CngAlgorithm.Sha256,
            };
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cng.Dispose();
            }
        }

        protected override void OnStart()
        {
            _publicKey = _cng.PublicKey.ToByteArray();
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
            var remoteKey = CngKey.Import(remotePublicKey, CngKeyBlobFormat.EccPublicBlob);
            _sharedKey = _cng.DeriveKeyMaterial(remoteKey);
        }
        
    }
}
