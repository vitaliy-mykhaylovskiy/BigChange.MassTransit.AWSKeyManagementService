using System.Linq;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache.CacheKeyGenerators
{
    public class CacheKeyGenerator : ICacheKeyGenerator
    {
        public string Generate(KeyCriteria criteria) => 
        string.Join("", new[]
            {
                criteria.KeyId
            }.Concat(criteria.Context.Select(x => x.Key + x.Value)));

    }

}