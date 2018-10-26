using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.KeyManagementService;
using Amazon.Runtime.SharedInterfaces;
using BigChange.MassTransit.AwsKeyManagementService.Tests.RabbitMq;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.Serialization;
using MassTransit.TestFramework.Messages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache.Tests.RabbitMq
{
    [TestFixture]
    public class Publishing_an_encrypted_message_to_an_endpoint_with_aws_kms_with_cache :
        RabbitMqTestFixture
    {
        private Task<ConsumeContext<PingMessage>> _handler;
        private readonly string _keyId;
        private readonly Mock<IAmazonKeyManagementService> _amazonKeyManagementService;

        public Publishing_an_encrypted_message_to_an_endpoint_with_aws_kms_with_cache()
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
                    x.GenerateDataKey(_keyId,
                        It.Is<Dictionary<string, string>>(d => d.Count == 0), "AES_256"))
                .Returns(new GenerateDataKeyResult {KeyCiphertext = keyCiphertext, KeyPlaintext = key});

            _amazonKeyManagementService.Setup(x => x.Decrypt(It.Is<byte[]>(d => d.SequenceEqual(keyCiphertext)),
                    It.Is<Dictionary<string, string>>(d => d.Count == 0)))
                .Returns(key);
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(_keyId, _amazonKeyManagementService.Object,
                Options.Create(new MemoryDistributedCacheOptions()), new AbsoluteExpirationRelativeToNowOptionsFactory(TimeSpan.FromMinutes(1)));

            base.ConfigureRabbitMqBus(configurator);
        }


        [Test]
        public async Task Should_succeed()
        {
            await Bus.Publish(new PingMessage());
            await Bus.Publish(new PingMessage());

            var received1 = await _handler;
            var received2 = await _handler;

            Assert.AreEqual(EncryptedMessageSerializerV2.EncryptedContentType, received1.ReceiveContext.ContentType);
            Assert.AreEqual(EncryptedMessageSerializerV2.EncryptedContentType, received2.ReceiveContext.ContentType);

            _amazonKeyManagementService.Verify(x =>
                x.GenerateDataKey(_keyId,
                    It.Is<Dictionary<string, string>>(d => d.Count == 0), "AES_256"), Times.Once);

            _amazonKeyManagementService.Verify(x => x.Decrypt(It.IsAny<byte[]>(),
                It.IsAny<Dictionary<string, string>>()), Times.Once);
        }
    }
}