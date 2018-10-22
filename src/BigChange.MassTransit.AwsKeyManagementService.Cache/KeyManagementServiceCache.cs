using System.Collections.Generic;
using System.Text;
using Amazon.Runtime.SharedInterfaces;
using BigChange.MassTransit.AwsKeyManagementService.Cache.CacheKeyGenerators;
using Microsoft.Extensions.Caching.Distributed;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
    public class KeyManagementServiceCache : IKeyManagementService
    {
        private readonly IKeyManagementService _keyManagementService;
        private readonly IDistributedCache _distributedCache;
        private readonly ICacheKeyGenerator _cacheKeyGenerator;

        public KeyManagementServiceCache(
            IKeyManagementService keyManagementService, 
            IDistributedCache distributedCache,
            ICacheKeyGenerator cacheKeyGenerator)
        {
            _distributedCache = distributedCache;
            _cacheKeyGenerator = cacheKeyGenerator;
            _keyManagementService = keyManagementService;
        }

        public byte[] Decrypt(byte[] ciphertextBlob, Dictionary<string, string> encryptionContext)
        {
            var key = _cacheKeyGenerator.Generate(new KeyCriteria 
            {
                KeyId = Encoding.UTF8.GetString(ciphertextBlob), 
                Context = encryptionContext
            });

            var cacheItem = _distributedCache.Get(key);

            if (cacheItem is null)
            {
                var item = _keyManagementService.Decrypt(ciphertextBlob, encryptionContext);

                _distributedCache.Set(key, item);

                return item;
            }

            return cacheItem;
        }

        public GenerateDataKeyResult GenerateDataKey(string keyId, Dictionary<string, string> encryptionContext, string keySpec)
        {
            var key = _cacheKeyGenerator.Generate(new KeyCriteria 
            {
                KeyId = keyId, 
                Context = encryptionContext
            });

            var cacheItem = _distributedCache.Get(key);

            if (cacheItem is null)
            {
                var item = _keyManagementService.GenerateDataKey(keyId, encryptionContext, keySpec);

                _distributedCache.Set(key, new GenerateDataKeyResultSerializable
                {
                    KeyCiphertext = item.KeyCiphertext,
                    KeyPlaintext = item.KeyPlaintext
                }.GetBytes());

                return item;
            }

            return GenerateDataKeyResultSerializable.FromBytes(cacheItem);
        }
    }
}