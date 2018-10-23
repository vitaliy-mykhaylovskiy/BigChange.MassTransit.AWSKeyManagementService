using System.Collections.Generic;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache.CacheKeyGenerators
{
    public interface ICacheKeyGenerator
	{
        string Generate(string keyId,  IReadOnlyDictionary<string, string> context);

        string Generate(byte[] ciphertextBlob,  IReadOnlyDictionary<string, string> context);
	}
}