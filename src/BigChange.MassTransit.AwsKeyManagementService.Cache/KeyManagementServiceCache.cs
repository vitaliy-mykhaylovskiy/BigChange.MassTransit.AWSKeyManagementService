using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime.SharedInterfaces;
using BigChange.MassTransit.AwsKeyManagementService.Cache.CacheKeyGenerators;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
    public class KeyManagementServiceCache : IKeyManagementService
    {
        private IKeyManagementService _keyManagementService;
        private readonly IDistributedCache _memoryCache;
        private readonly ICacheKeyGenerator _cacheKeyGenerator;

        public KeyManagementServiceCache(
            IKeyManagementService keyManagementService, 
            IDistributedCache memoryCache,
            ICacheKeyGenerator cacheKeyGenerator)
        {
            _memoryCache = memoryCache;
            _cacheKeyGenerator = cacheKeyGenerator;
            _keyManagementService = keyManagementService;
        }

        public byte[] Decrypt(byte[] ciphertextBlob, Dictionary<string, string> encryptionContext)
        {
            var key = _cacheKeyGenerator.Generate(new KeyCriteria 
            {
                KeyId = System.Text.UTF8Encoding.UTF8.GetString(ciphertextBlob), 
                Context = encryptionContext
            });

            var cacheItem = _memoryCache.Get(key);

            if (cacheItem is null)
            {
                var item = _keyManagementService.Decrypt(ciphertextBlob, encryptionContext);

                _memoryCache.Set(key, item);
                return item;
            }

            return cacheItem;
        }

        public GenerateDataKeyResult GenerateDataKey(string keyID, Dictionary<string, string> encryptionContext, string keySpec)
        {
            var key = _cacheKeyGenerator.Generate(new KeyCriteria 
            {
                KeyId = keyID, 
                Context = encryptionContext
            });

            var cacheItem = _memoryCache.Get(key);

            if (cacheItem is null)
            {
                var item = _keyManagementService.GenerateDataKey(keyID, encryptionContext, keySpec);

                _memoryCache.Set(key, item.ToByteArray());
                return item;
            }

            return new GenerateDataKeyResult().FromByteArray(cacheItem);
        }
    }
}