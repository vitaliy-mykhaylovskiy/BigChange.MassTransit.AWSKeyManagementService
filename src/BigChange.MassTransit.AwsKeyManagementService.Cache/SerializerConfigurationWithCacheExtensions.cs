using System.Collections.Generic;
using Amazon;
using Amazon.KeyManagementService;
using MassTransit;
using MassTransit.Serialization;
using BigChange.MassTransit.AwsKeyManagementService;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using BigChange.MassTransit.AwsKeyManagementService.Cache.CacheKeyGenerators;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
    public static class SerializerConfigurationWithCacheExtensions
    {
        public static void UseAwsKeyManagementServiceSerializer(this IBusFactoryConfigurator configurator,
            IAmazonKeyManagementService keyManagementService, 
            IEncryptionContextBuilder encryptionContextBuilder,
            string kmsKeyId)
        {
        }

        public static void UseAwsKeyManagementServiceSerializerWithCache(this IBusFactoryConfigurator configurator,
            IKeyManagementService amazonKeyManagementService, 
            IEncryptionContextBuilder encryptionContextBuilder,
            string kmsKeyId,
            IDistributedCache distributedCache,
            ICacheKeyGenerator cacheKeyGenerator)
        {
            var kmsSecureKeyProvider =
                new KmsSecureKeyProvider(
                    new KeyManagementServiceCache(amazonKeyManagementService, distributedCache, cacheKeyGenerator),
                    encryptionContextBuilder, 
                    kmsKeyId);
            var aesCryptoStreamProvider = new AesCryptoStreamProviderV2(kmsSecureKeyProvider);
            configurator.SetMessageSerializer(() => new EncryptedMessageSerializerV2(aesCryptoStreamProvider));

            configurator.AddMessageDeserializer(EncryptedMessageSerializer.EncryptedContentType,
                () => new EncryptedMessageDeserializerV2(BsonMessageSerializer.Deserializer, aesCryptoStreamProvider));
        }

        public static void UseAwsKeyManagementServiceSerializerWithCache(this IReceiveEndpointConfigurator configurator,
            IKeyManagementService amazonKeyManagementService, 
            IEncryptionContextBuilder encryptionContextBuilder,
            string kmsKeyId,
            IDistributedCache distributedCache,
            ICacheKeyGenerator cacheKeyGenerator)
        {
            var kmsSecureKeyProvider =
                new KmsSecureKeyProvider(
                    new KeyManagementServiceCache(amazonKeyManagementService, distributedCache, cacheKeyGenerator),
                    encryptionContextBuilder, 
                    kmsKeyId);

            var aesCryptoStreamProvider = new AesCryptoStreamProviderV2(kmsSecureKeyProvider);
            configurator.SetMessageSerializer(() => new EncryptedMessageSerializerV2(aesCryptoStreamProvider));

            configurator.AddMessageDeserializer(EncryptedMessageSerializer.EncryptedContentType,
                () => new EncryptedMessageDeserializerV2(BsonMessageSerializer.Deserializer, aesCryptoStreamProvider));
        }
    }
}