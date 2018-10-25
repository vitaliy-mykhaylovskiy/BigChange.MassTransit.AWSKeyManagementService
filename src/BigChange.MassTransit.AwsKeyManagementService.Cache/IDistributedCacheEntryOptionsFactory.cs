using Microsoft.Extensions.Caching.Distributed;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
    public interface IDistributedCacheEntryOptionsFactory
    {
        DistributedCacheEntryOptions Create();
    }
}