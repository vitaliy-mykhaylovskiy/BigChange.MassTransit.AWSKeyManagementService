using Amazon.Runtime.SharedInterfaces;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
    public interface ICacheValueConverter
    {
        GenerateDataKeyResult Convert(byte[] cacheValue);
        byte[] Convert(GenerateDataKeyResult result);
    }
}