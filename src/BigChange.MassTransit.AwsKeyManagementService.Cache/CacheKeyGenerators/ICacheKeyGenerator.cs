namespace BigChange.MassTransit.AwsKeyManagementService.Cache.CacheKeyGenerators
{
    public interface ICacheKeyGenerator
    {
        string Generate(DataKeyIdentifier key);

        string Generate(DecryptIdentifier identifier);
    }
}