using System;
using System.Security.Cryptography;

namespace FWF.KeyExchange
{
    internal class KeyExchangeProvider : Startable, IKeyExchangeProvider, IDisposable
    {

        private byte[] _publicKey;
        private byte[] _sharedKey;

        private readonly ECDiffieHellmanCng _cng;

        public KeyExchangeProvider()
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

        public KeyExchangeBitLength BitLength
        {
            get
            {
                if (_cng.HashAlgorithm == CngAlgorithm.Sha256)
                {
                    return KeyExchangeBitLength.Hash256;
                }
                if (_cng.HashAlgorithm == CngAlgorithm.Sha512)
                {
                    return KeyExchangeBitLength.Hash512;
                }

                throw new InvalidOperationException("Unable to determine bit length");
            }
            set
            {
                if (value == KeyExchangeBitLength.Hash256)
                {
                    _cng.HashAlgorithm = CngAlgorithm.Sha256;
                }
                if (value == KeyExchangeBitLength.Hash512)
                {
                    _cng.HashAlgorithm = CngAlgorithm.Sha512;
                }

                throw new InvalidOperationException("Unable to set bit length");
            }
        }

        public byte[] PublicKey
        {
            get
            {
                return _publicKey;
            }
        }

        public void ConfigureEndpointExchange(string endpointId, byte[] remotePublicKey)
        {
            var remoteKey = CngKey.Import(remotePublicKey, CngKeyBlobFormat.EccPublicBlob);
            _sharedKey = _cng.DeriveKeyMaterial(remoteKey);
        }

        public bool IsEndpointConfigured(string endpointId)
        {
            return false;
        }


        public byte[] SharedKey
        {
            get
            {
                return _sharedKey;
            }
        }
        
    }
}
