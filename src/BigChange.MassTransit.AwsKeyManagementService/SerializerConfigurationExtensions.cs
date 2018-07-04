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

		public static void UseAwsKeyManagementServiceSerializer(this IReceiveEndpointConfigurator configurator, string keyId)
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

		public static void UseAwsKeyManagementServiceSerializer(this IBusFactoryConfigurator configurator, IAmazonKeyManagementService amazonKeyManagementService, string keyId)
		{
			configurator.UseAwsKeyManagementServiceSerializer(amazonKeyManagementService, new MessageEncryptionContextBuilder(), keyId);
		}

		public static void UseAwsKeyManagementServiceSerializer(this IReceiveEndpointConfigurator configurator, IAmazonKeyManagementService amazonKeyManagementService, string keyId)
		{
			configurator.UseAwsKeyManagementServiceSerializer(amazonKeyManagementService, new MessageEncryptionContextBuilder(), keyId);
		}

		public static void UseAwsKeyManagementServiceSerializer(this IBusFactoryConfigurator configurator, IAmazonKeyManagementService amazonKeyManagementService, IEncryptionContextBuilder encryptionContextBuilder, string keyId)
		{
			configurator.SetMessageSerializer(() => new AwsKeyManagementServiceMessageSerializer(amazonKeyManagementService, encryptionContextBuilder, keyId));

			configurator.AddMessageDeserializer(AwsKeyManagementServiceMessageSerializer.AwsKmsEncryptedContentType,
				() => new AwsKeyManagementServiceMessageDeserializer(BsonMessageSerializer.Deserializer, amazonKeyManagementService, encryptionContextBuilder));
		}

		public static void UseAwsKeyManagementServiceSerializer(this IReceiveEndpointConfigurator configurator, IAmazonKeyManagementService amazonKeyManagementService, IEncryptionContextBuilder encryptionContextBuilder, string keyId)
		{
			configurator.SetMessageSerializer(() => new AwsKeyManagementServiceMessageSerializer(amazonKeyManagementService, encryptionContextBuilder, keyId));

			configurator.AddMessageDeserializer(AwsKeyManagementServiceMessageSerializer.AwsKmsEncryptedContentType,
				() => new AwsKeyManagementServiceMessageDeserializer(BsonMessageSerializer.Deserializer, amazonKeyManagementService, encryptionContextBuilder));
		}
	}
}