using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.KeyManagementService;
using Amazon.Runtime.SharedInterfaces;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.TestFramework.Messages;
using Moq;
using NUnit.Framework;

namespace BigChange.MassTransit.AwsKeyManagementService.Tests.RabbitMq
{
	[TestFixture]
	public class Publishing_an_encrypted_message_to_an_endpoint_with_aws_kms :
		RabbitMqTestFixture
	{
		private Task<ConsumeContext<PingMessage>> _handler;
		private readonly string _keyId;
		private readonly Mock<IAmazonKeyManagementService> _amazonKeyManagementService;

		public Publishing_an_encrypted_message_to_an_endpoint_with_aws_kms()
		{
			_keyId = "alias/masstransit";
			var keyCiphertext = Guid.NewGuid().ToByteArray();
			var key = new byte[]
			{
				31, 182, 254, 29, 98, 114, 85, 168, 176, 48, 113,
				206, 198, 176, 181, 125, 106, 134, 98, 217, 113,
				158, 88, 75, 118, 223, 117, 160, 224, 1, 47, 162
			};
			
			_amazonKeyManagementService = new Mock<IAmazonKeyManagementService>();
			_amazonKeyManagementService.Setup(x =>
					x.GenerateDataKey(_keyId, It.Is<Dictionary<string, string>>(d => d.Count == 1 && d.ContainsKey("message_id")), "AES_256"))
				.Returns(new GenerateDataKeyResult{KeyCiphertext = keyCiphertext, KeyPlaintext = key});

			_amazonKeyManagementService.Setup(x => x.Decrypt(It.Is<byte[]>(d => d.SequenceEqual(keyCiphertext)),
					It.Is<Dictionary<string, string>>(d => d.Count == 1 && d.ContainsKey("message_id"))))
				.Returns(key);
		}

		protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
		{
			_handler = Handled<PingMessage>(configurator);
		}

		protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
		{
			configurator.UseAwsKeyManagementServiceSerializer(_amazonKeyManagementService.Object, _keyId);

			base.ConfigureRabbitMqBus(configurator);
		}


		[Test]
		public async Task Should_succeed()
		{
			await Bus.Publish(new PingMessage());

			ConsumeContext<PingMessage> received = await _handler;

			Assert.AreEqual(EncryptedMessageSerializer.EncryptedContentType, received.ReceiveContext.ContentType);
		}
	}
}
