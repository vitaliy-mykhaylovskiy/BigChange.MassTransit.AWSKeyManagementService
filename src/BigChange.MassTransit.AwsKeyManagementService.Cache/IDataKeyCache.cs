using Amazon.Runtime.SharedInterfaces;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
    public interface IDataKeyCache
    {
        GenerateDataKeyResult Get(DataKeyIdentifier key);

        void Set(DataKeyIdentifier key, GenerateDataKeyResult item);
    }
}