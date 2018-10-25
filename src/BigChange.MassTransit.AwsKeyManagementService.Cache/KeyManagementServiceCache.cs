using System.Collections.Generic;
using System.Text;
using Amazon.Runtime.SharedInterfaces;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
    public class KeyManagementServiceCache : IKeyManagementService
    {
        private readonly IKeyManagementService _keyManagementService;
        private readonly IDataKeyCache _dataKeyCache;
        private readonly IDecryptKeyCache _decryptKeyCache;

        public KeyManagementServiceCache(
            IKeyManagementService keyManagementService,
            IDataKeyCache dataKeyCache,
            IDecryptKeyCache decryptKeyCache)
        {
            _dataKeyCache = dataKeyCache;
            _decryptKeyCache = decryptKeyCache;
            _keyManagementService = keyManagementService;
        }

        public byte[] Decrypt(byte[] ciphertextBlob, Dictionary<string, string> encryptionContext)
        {
            var identifier = new DecryptIdentifier(ciphertextBlob, encryptionContext);

            var cacheItem = _decryptKeyCache.Get(identifier);

            if (cacheItem is null)
            {
                var item = _keyManagementService.Decrypt(ciphertextBlob, encryptionContext);

                _decryptKeyCache.Set(identifier, item);

                return item;
            }

            return cacheItem;
        }

        public GenerateDataKeyResult GenerateDataKey(string keyId, Dictionary<string, string> encryptionContext,
            string keySpec)
        {
            var identifier = new DataKeyIdentifier(keyId, encryptionContext);

            var cacheItem = _dataKeyCache.Get(identifier);

            if (cacheItem is null)
            {
                var item = _keyManagementService.GenerateDataKey(keyId, encryptionContext, keySpec);

                return item;
            }

            return cacheItem;
        }
    }
}