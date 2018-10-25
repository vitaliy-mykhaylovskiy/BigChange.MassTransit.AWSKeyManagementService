using Amazon.Runtime.SharedInterfaces;
using BigChange.MassTransit.AwsKeyManagementService.Cache.CacheKeyGenerators;
using Microsoft.Extensions.Caching.Distributed;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
    public class DataKeyCache : IDataKeyCache
    {
        private readonly ICacheKeyGenerator _cacheKeyGenerator;
        private readonly IDistributedCache _distributedCache;
        private readonly ICacheValueConverter _cacheValueConverter;

        public DataKeyCache(ICacheKeyGenerator cacheKeyGenerator,
            IDistributedCache distributedCache, ICacheValueConverter cacheValueConverter)
        {
            _cacheKeyGenerator = cacheKeyGenerator;
            _distributedCache = distributedCache;
            _cacheValueConverter = cacheValueConverter;
        }

        public GenerateDataKeyResult Get(DataKeyIdentifier key)
        {
            var cacheKey = _cacheKeyGenerator.Generate(key);

            var cacheItem = _distributedCache.Get(cacheKey);

            if (cacheItem is null)
            {
                return null;
            }

            return _cacheValueConverter.Convert(cacheItem);
        }

        public void Set(DataKeyIdentifier key, GenerateDataKeyResult item)
        {
            var cacheKey = _cacheKeyGenerator.Generate(key);

            var cacheValue = _cacheValueConverter.Convert(item);

            _distributedCache.Set(cacheKey, cacheValue);
        }
    }
}