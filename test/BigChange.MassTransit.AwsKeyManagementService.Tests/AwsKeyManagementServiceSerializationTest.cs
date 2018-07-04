using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Amazon.KeyManagementService;
using Amazon.Runtime.SharedInterfaces;
using MassTransit;
using MassTransit.Pipeline.Observables;
using MassTransit.Serialization;
using MassTransit.TestFramework;
using MassTransit.Transports.InMemory.Contexts;
using MassTransit.Transports.InMemory.Fabric;
using MassTransit.Util;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace BigChange.MassTransit.AwsKeyManagementService.Tests
{
	public abstract class AwsKeyManagementServiceSerializationTest :
		InMemoryTestFixture
	{
		protected IMessageDeserializer Deserializer;
		protected IMessageSerializer Serializer;
		readonly Uri _sourceAddress = new Uri("loopback://localhost/source");
		readonly Uri _destinationAddress = new Uri("loopback://localhost/destination");
		readonly Uri _responseAddress = new Uri("loopback://localhost/response");
		readonly Uri _faultAddress = new Uri("loopback://localhost/fault");
		protected readonly Guid _requestId = Guid.NewGuid();
		private byte[] _keyCiphertext;

		[OneTimeSetUp]
		public void SetupSerializationTest()
		{
			_keyCiphertext = Guid.NewGuid().ToByteArray();
			var key = new byte[]
			{
				31, 182, 254, 29, 98, 114, 85, 168, 176, 48, 113,
				206, 198, 176, 181, 125, 106, 134, 98, 217, 113,
				158, 88, 75, 118, 223, 117, 160, 224, 1, 47, 162
			};

			var amazonKeyManagementService = new Mock<IAmazonKeyManagementService>();
			amazonKeyManagementService.Setup(x =>
					x.GenerateDataKey("abc", It.Is<Dictionary<string, string>>(d => d.Count == 1 && d.ContainsKey("message_id")), "AES_256"))
				.Returns(new GenerateDataKeyResult { KeyCiphertext = _keyCiphertext, KeyPlaintext = key });

			amazonKeyManagementService.Setup(x => x.Decrypt(It.Is<byte[]>(d => d.SequenceEqual(_keyCiphertext)),
					It.Is<Dictionary<string, string>>(d => d.Count == 1 && d.ContainsKey("message_id"))))
				.Returns(key);

			Serializer = new AwsKeyManagementServiceMessageSerializer(amazonKeyManagementService.Object, "abc");
			Deserializer = new AwsKeyManagementServiceMessageDeserializer(BsonMessageSerializer.Deserializer, amazonKeyManagementService.Object);
		}

		protected T SerializeAndReturn<T>(T obj)
			where T : class
		{

			var serializedMessageData = Serialize(obj);

			return Return<T>(serializedMessageData);
		}

		protected byte[] Serialize<T>(T obj)
			where T : class
		{
			using (var output = new MemoryStream())
			{
				var sendContext = new InMemorySendContext<T>(obj);

				sendContext.SourceAddress = _sourceAddress;
				sendContext.DestinationAddress = _destinationAddress;
				sendContext.FaultAddress = _faultAddress;
				sendContext.ResponseAddress = _responseAddress;
				sendContext.RequestId = _requestId;

				Serializer.Serialize(output, sendContext);

				byte[] serializedMessageData = output.ToArray();

				Trace.WriteLine(Encoding.UTF8.GetString(serializedMessageData));
				return serializedMessageData;
			}
		}

		protected T Return<T>(byte[] serializedMessageData)
			where T : class
		{
			var message = new InMemoryTransportMessage(Guid.NewGuid(), serializedMessageData, Serializer.ContentType.MediaType, TypeMetadataCache<T>.ShortName);
			message.Headers.Add(AwsKeyManagementServiceMessageSerializer.DataKeyCiphertextHeader, Convert.ToBase64String(_keyCiphertext));

			var receiveContext = new InMemoryReceiveContext(new Uri("loopback://localhost/input_queue"), message, new ReceiveObservable(), null);

			ConsumeContext consumeContext = Deserializer.Deserialize(receiveContext);

			ConsumeContext<T> messageContext;
			consumeContext.TryGetMessage(out messageContext);

			messageContext.ShouldNotBe(null);

			messageContext.SourceAddress.ShouldBe(_sourceAddress);
			messageContext.DestinationAddress.ShouldBe(_destinationAddress);
			messageContext.FaultAddress.ShouldBe(_faultAddress);
			messageContext.ResponseAddress.ShouldBe(_responseAddress);
			messageContext.RequestId.HasValue.ShouldBe(true);
			messageContext.RequestId.Value.ShouldBe(_requestId);

			return messageContext.Message;
		}

		protected virtual void TestSerialization<T>(T message)
			where T : class
		{
			T result = SerializeAndReturn(message);

			message.Equals(result).ShouldBe(true);
		}
	}
}