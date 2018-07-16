using System.Collections.Generic;
using Amazon;
using Amazon.KeyManagementService;
using MassTransit;
using MassTransit.Serialization;

namespace BigChange.MassTransit.AwsKeyManagementService
{
    public static class SerializerConfigurationExtensions
    {
        public static void UseAwsKeyManagementServiceSerializer(this IBusFactoryConfigurator configurator, string keyId)
        {
            configurator.UseAwsKeyManagementServiceSerializer(new AmazonKeyManagementServiceClient(), keyId);
        }

        public static void UseAwsKeyManagementServiceSerializer(this IReceiveEndpointConfigurator configurator,
            string keyId)
        {
            configurator.UseAwsKeyManagementServiceSerializer(new AmazonKeyManagementServiceClient(), keyId);
        }

        public static void UseAwsKeyManagementServiceSerializer(this IBusFactoryConfigurator configurator,
            RegionEndpoint region, string keyId)
        {
            configurator.UseAwsKeyManagementServiceSerializer(new AmazonKeyManagementServiceClient(region), keyId);
        }

        public static void UseAwsKeyManagementServiceSerializer(this IReceiveEndpointConfigurator configurator,
            RegionEndpoint region, string keyId)
        {
            configurator.UseAwsKeyManagementServiceSerializer(new AmazonKeyManagementServiceClient(region), keyId);
        }

        public static void UseAwsKeyManagementServiceSerializer(this IBusFactoryConfigurator configurator,
            IAmazonKeyManagementService amazonKeyManagementService, string keyId)
        {
            configurator.UseAwsKeyManagementServiceSerializer(amazonKeyManagementService,
                new MessageEncryptionContextBuilder(), keyId);
        }

        public static void UseAwsKeyManagementServiceSerializer(this IReceiveEndpointConfigurator configurator,
            IAmazonKeyManagementService amazonKeyManagementService, string keyId)
        {
            configurator.UseAwsKeyManagementServiceSerializer(amazonKeyManagementService,
                new MessageEncryptionContextBuilder(), keyId);
        }

        public static void UseAwsKeyManagementServiceSerializer(this IBusFactoryConfigurator configurator,
            IEncryptionContextBuilder encryptionContextBuilder,
            string kmsKeyId)
        {
            configurator.UseAwsKeyManagementServiceSerializer(new AmazonKeyManagementServiceClient(),
                encryptionContextBuilder, kmsKeyId);
        }

        public static void UseAwsKeyManagementServiceSerializer(this IReceiveEndpointConfigurator configurator,
            IEncryptionContextBuilder encryptionContextBuilder,
            string kmsKeyId)
        {
            configurator.UseAwsKeyManagementServiceSerializer(new AmazonKeyManagementServiceClient(),
                encryptionContextBuilder, kmsKeyId);
        }

        public static void UseAwsKeyManagementServiceSerializer(this IBusFactoryConfigurator configurator,
            IAmazonKeyManagementService amazonKeyManagementService, IEncryptionContextBuilder encryptionContextBuilder,
            string kmsKeyId)
        {
            var kmsSecureKeyProvider =
                new KmsSecureKeyProvider(amazonKeyManagementService, encryptionContextBuilder, kmsKeyId);
            var aesCryptoStreamProvider = new AesCryptoStreamProviderV2(kmsSecureKeyProvider);

            configurator.SetMessageSerializer(() => new EncryptedMessageSerializerV2(aesCryptoStreamProvider));

            configurator.AddMessageDeserializer(EncryptedMessageSerializer.EncryptedContentType,
                () => new EncryptedMessageDeserializerV2(BsonMessageSerializer.Deserializer, aesCryptoStreamProvider));
        }

        public static void UseAwsKeyManagementServiceSerializer(this IReceiveEndpointConfigurator configurator,
            IAmazonKeyManagementService amazonKeyManagementService, IEncryptionContextBuilder encryptionContextBuilder,
            string kmsKeyId)
        {
            var kmsSecureKeyProvider =
                new KmsSecureKeyProvider(amazonKeyManagementService, encryptionContextBuilder, kmsKeyId);
            var aesCryptoStreamProvider = new AesCryptoStreamProviderV2(kmsSecureKeyProvider);
            configurator.SetMessageSerializer(() => new EncryptedMessageSerializerV2(aesCryptoStreamProvider));

            configurator.AddMessageDeserializer(EncryptedMessageSerializer.EncryptedContentType,
                () => new EncryptedMessageDeserializerV2(BsonMessageSerializer.Deserializer, aesCryptoStreamProvider));
        }
    }
}