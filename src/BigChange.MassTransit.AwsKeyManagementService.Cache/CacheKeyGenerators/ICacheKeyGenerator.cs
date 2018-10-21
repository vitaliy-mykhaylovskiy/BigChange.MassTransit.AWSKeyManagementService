namespace BigChange.MassTransit.AwsKeyManagementService.Cache.CacheKeyGenerators
{
    public interface ICacheKeyGenerator
    {
        string Generate(KeyCriteria criteria);
    }
}