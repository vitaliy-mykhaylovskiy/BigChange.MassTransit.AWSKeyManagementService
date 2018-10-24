using System;
using System.Collections.Generic;
using System.Linq;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache.CacheKeyGenerators
{
    public class CacheKeyGenerator : ICacheKeyGenerator
    {
        public string Generate(string keyId, IReadOnlyDictionary<string, string> context)
        {
            return keyId + BuildContextKeyString(context);
        }

        public string Generate(byte[] ciphertextBlob, IReadOnlyDictionary<string, string> context)
        {
            return Convert.ToBase64String(ciphertextBlob) + BuildContextKeyString(context);
        }

        private static string BuildContextKeyString(IReadOnlyDictionary<string, string> context)
        {
            return string.Concat(context.Select(x => x.Key + x.Value));
        }
    }
}