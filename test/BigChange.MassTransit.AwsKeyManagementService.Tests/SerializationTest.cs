using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using GreenPipes;
using MassTransit;
using MassTransit.Pipeline.Observables;
using MassTransit.Serialization;
using MassTransit.TestFramework;
using MassTransit.Transports.InMemory.Contexts;
using MassTransit.Transports.InMemory.Fabric;
using MassTransit.Util;
using NUnit.Framework;
using Shouldly;

namespace BigChange.MassTransit.AwsKeyManagementService.Tests
{
    public abstract class SerializationTest :
        InMemoryTestFixture
    {
        private readonly Uri _destinationAddress = new Uri("loopback://localhost/destination");
        private readonly Uri _faultAddress = new Uri("loopback://localhost/fault");
        protected readonly Guid _requestId = Guid.NewGuid();
        private readonly Uri _responseAddress = new Uri("loopback://localhost/response");
        private readonly Uri _sourceAddress = new Uri("loopback://localhost/source");
        private byte[] _keyCiphertext;
        protected IMessageDeserializer Deserializer;
        protected IMessageSerializer Serializer;

        [OneTimeSetUp]
        public void SetupSerializationTest()
        {
            var keyProvider = new TestSecureKeyProvider();
            var cryptoStreamProvider = new AesCryptoStreamProvider(keyProvider);

            Serializer = new EncryptedMessageSerializer(cryptoStreamProvider);
            Deserializer = new EncryptedMessageDeserializer(BsonMessageSerializer.Deserializer, cryptoStreamProvider);
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

                var serializedMessageData = output.ToArray();

                Trace.WriteLine(Encoding.UTF8.GetString(serializedMessageData));
                return serializedMessageData;
            }
        }

        protected T Return<T>(byte[] serializedMessageData)
            where T : class
        {
            var message = new InMemoryTransportMessage(Guid.NewGuid(), serializedMessageData,
                Serializer.ContentType.MediaType, TypeMetadataCache<T>.ShortName);

            var receiveContext = new InMemoryReceiveContext(new Uri("loopback://localhost/input_queue"), message,
                new ReceiveObservable(), null);

            var consumeContext = Deserializer.Deserialize(receiveContext);

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
            var result = SerializeAndReturn(message);

            message.Equals(result).ShouldBe(true);
        }
    }

    public class TestSecureKeyProvider : ISecureKeyProvider
    {
        private static readonly byte[] Key =
        {
            31, 182, 254, 29, 98, 114, 85, 168, 176, 48, 113,
            206, 198, 176, 181, 125, 106, 134, 98, 217, 113,
            158, 88, 75, 118, 223, 117, 160, 224, 1, 47, 162
        };

        public void Probe(ProbeContext context)
        {
        }

        public byte[] GetKey(ReceiveContext receiveContext)
        {
            return Key;
        }

        public byte[] GetKey(SendContext sendContext)
        {
            return Key;
        }
    }
}