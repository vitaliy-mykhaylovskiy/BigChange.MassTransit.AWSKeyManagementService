using System;
using System.Collections.Generic;
using System.Linq;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache.CacheKeyGenerators
{
    public class CacheKeyGenerator : ICacheKeyGenerator
    {
        public string Generate(DataKeyIdentifier key)
        {
            return key.KeyId + BuildContextKeyString(key.EncryptionContext);
        }

        public string Generate(DecryptIdentifier identifier)
        {
            return Convert.ToBase64String(identifier.CiphertextBlob) + BuildContextKeyString(identifier.EncryptionContext);
        }

        private static string BuildContextKeyString(IReadOnlyDictionary<string, string> context)
        {
            return string.Concat(context.Select(x => x.Key + x.Value));
        }
    }
}