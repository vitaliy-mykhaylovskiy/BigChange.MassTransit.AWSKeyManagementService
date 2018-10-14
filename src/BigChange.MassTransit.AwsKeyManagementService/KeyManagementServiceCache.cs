using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime.SharedInterfaces;
using Microsoft.Extensions.Caching.Memory;

namespace BigChange.MassTransit.AwsKeyManagementService
{
    public class KeyManagementServiceCache : IKeyManagementService
    {
        private IKeyManagementService _keyManagementService;
        private readonly IMemoryCache _memoryCache;

        public KeyManagementServiceCache(IKeyManagementService keyManagementService, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _keyManagementService = keyManagementService;
        }

        public byte[] Decrypt(byte[] ciphertextBlob, Dictionary<string, string> encryptionContext)
        {
            return _memoryCache.GetOrCreate(CacheKey(ciphertextBlob, encryptionContext),
                x => _keyManagementService.Decrypt(ciphertextBlob, encryptionContext));
        }

        public GenerateDataKeyResult GenerateDataKey(string keyID, Dictionary<string, string> encryptionContext, string keySpec)
        {
            return _memoryCache.GetOrCreate(CacheKey(keyID, encryptionContext),
                 x => _keyManagementService.GenerateDataKey(keyID, encryptionContext, keySpec));
        }

        private string CacheKey(string keyId, Dictionary<string, string> context)
            => string.Join("", new[] { keyId }.Concat(context.Select(x => x.Key + x.Value)));
            
        private string CacheKey(byte[] ciphertextBlob, Dictionary<string, string> encryptionContext)
            => CacheKey(System.Text.Encoding.UTF8.GetString(ciphertextBlob), encryptionContext);
    }
}