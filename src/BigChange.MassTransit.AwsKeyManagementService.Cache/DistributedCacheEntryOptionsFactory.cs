using System;
using Microsoft.Extensions.Caching.Distributed;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
    public class AbsoluteExpirationRelativeToNowOptionsFactory : IDistributedCacheEntryOptionsFactory
    {
        private readonly TimeSpan _absoluteExpirationRelativeToNow;

        public AbsoluteExpirationRelativeToNowOptionsFactory(TimeSpan absoluteExpirationRelativeToNow)
        {
            _absoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
        }

        public DistributedCacheEntryOptions Create(CacheItemType type)
        {
            return new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _absoluteExpirationRelativeToNow;
            };
        }
    }
}