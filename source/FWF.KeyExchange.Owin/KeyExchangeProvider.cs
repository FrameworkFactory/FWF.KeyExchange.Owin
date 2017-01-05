using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using FWF.KeyExchange.Owin.Logging;

namespace FWF.KeyExchange.Owin
{
    internal class KeyExchangeProvider : IKeyExchangeProvider, IDisposable
    {

        private readonly ECDiffieHellmanCng _cng;
        private readonly IDictionary<string, byte[]> _endpoints = new Dictionary<string, byte[]>(); 

        private readonly ICache _cache;
        private readonly ILog _log;

        public KeyExchangeProvider(
            ICache cache,
            ILogFactory logFactory
            )
        {
            _cache = cache;

            _log = logFactory.CreateForType(this);

            var keyName = "ECDH";

            CngKey cngKey;

            if (CngKey.Exists(keyName, CngProvider.MicrosoftSoftwareKeyStorageProvider, CngKeyOpenOptions.Silent))
            {
                cngKey = CngKey.Open(keyName, CngProvider.MicrosoftSoftwareKeyStorageProvider, CngKeyOpenOptions.Silent);
            }
            else
            {
                cngKey = CngKey.Create(
                    CngAlgorithm.ECDiffieHellmanP256,
                    keyName,
                    new CngKeyCreationParameters
                    {
                        ExportPolicy = CngExportPolicies.AllowPlaintextExport,
                        KeyCreationOptions = CngKeyCreationOptions.None,
                        KeyUsage = CngKeyUsages.AllUsages,
                        Provider = CngProvider.MicrosoftSoftwareKeyStorageProvider,
                        UIPolicy = new CngUIPolicy(CngUIProtectionLevels.None),
                    }
                    );
            }

            _cng = new ECDiffieHellmanCng(cngKey)
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
                _endpoints.Clear();
                _cng.Dispose();
            }
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
                return _cng.PublicKey.ToByteArray();
            }
        }

        public void ConfigureEndpointExchange(string endpointId, byte[] remotePublicKey)
        {
            if (endpointId.IsMissing())
            {
                throw new ArgumentNullException("endpointId");
            }
            if (ReferenceEquals(remotePublicKey, null) || remotePublicKey.Length == 0)
            {
                throw new ArgumentNullException("remotePublicKey");
            }

            // Generate a shared key from the remote public key
            var remoteKey = CngKey.Import(remotePublicKey, CngKeyBlobFormat.EccPublicBlob);
            var sharedKey = _cng.DeriveKeyMaterial(remoteKey);
            
            // Save the shared key
            _endpoints[endpointId] = sharedKey;

            // 
            _cache.Push(endpointId, sharedKey);
        }

        public bool IsEndpointConfigured(string endpointId)
        {
            return _endpoints.ContainsKey(endpointId);
        }

        public byte[] GetEndpointSharedKey(string endpointId)
        {
            if (_endpoints.ContainsKey(endpointId))
            {
                return _endpoints[endpointId];
            }

            var sharedKey = _cache.Fetch(endpointId);

            if (!ReferenceEquals(sharedKey, null))
            {
                _endpoints[endpointId] = sharedKey;
            }

            return sharedKey;
        }
        
    }
}
