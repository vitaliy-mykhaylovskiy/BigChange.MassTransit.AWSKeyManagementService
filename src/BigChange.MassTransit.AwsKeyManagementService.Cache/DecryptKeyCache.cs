using BigChange.MassTransit.AwsKeyManagementService.Cache.CacheKeyGenerators;
using Microsoft.Extensions.Caching.Distributed;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
    public class DecryptKeyCache : IDecryptKeyCache
    {
        private readonly ICacheKeyGenerator _cacheKeyGenerator;
        private readonly IDistributedCache _distributedCache;

        public DecryptKeyCache(ICacheKeyGenerator cacheKeyGenerator, IDistributedCache distributedCache)
        {
            _cacheKeyGenerator = cacheKeyGenerator;
            _distributedCache = distributedCache;
        }

        public byte[] Get(DecryptIdentifier identifier)
        {
            var cacheKey = _cacheKeyGenerator.Generate(identifier);

            var cacheItem = _distributedCache.Get(cacheKey);

            return cacheItem;
        }

        public void Set(DecryptIdentifier identifier, byte[] item)
        {
            var cacheKey = _cacheKeyGenerator.Generate(identifier);

            _distributedCache.Set(cacheKey, item);
        }
    }
}