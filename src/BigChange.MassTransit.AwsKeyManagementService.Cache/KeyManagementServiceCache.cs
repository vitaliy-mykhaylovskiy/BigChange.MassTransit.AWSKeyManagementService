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
	    private readonly ICacheValueConverter _cacheValueConverter;

	    public KeyManagementServiceCache(
            IKeyManagementService keyManagementService, 
            IDistributedCache distributedCache,
            ICacheKeyGenerator cacheKeyGenerator,
	        ICacheValueConverter cacheValueConverter)
        {
            _distributedCache = distributedCache;
            _cacheKeyGenerator = cacheKeyGenerator;
	        _cacheValueConverter = cacheValueConverter;
	        _keyManagementService = keyManagementService;
        }

        public byte[] Decrypt(byte[] ciphertextBlob, Dictionary<string, string> encryptionContext)
        {
            var key = _cacheKeyGenerator.Generate(ciphertextBlob, encryptionContext);

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
            var key = _cacheKeyGenerator.Generate(keyId, encryptionContext);

            var cacheItem = _distributedCache.Get(key);

            if (cacheItem is null)
            {
                var item = _keyManagementService.GenerateDataKey(keyId, encryptionContext, keySpec);

	            var cacheValue = _cacheValueConverter.Convert(item);

				_distributedCache.Set(key, cacheValue);

                return item;
            }

	        return _cacheValueConverter.Convert(cacheItem);
        }
    }
}