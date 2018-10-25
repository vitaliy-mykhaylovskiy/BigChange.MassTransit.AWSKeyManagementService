namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
    public interface IDecryptKeyCache
    {
        byte[] Get(DecryptIdentifier identifier);

        void Set(DecryptIdentifier identifier, byte[] item);
    }
}