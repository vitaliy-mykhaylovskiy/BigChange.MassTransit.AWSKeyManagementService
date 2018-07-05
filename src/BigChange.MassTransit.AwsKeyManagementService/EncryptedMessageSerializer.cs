using System.IO;
using System.Net.Mime;
using MassTransit;
using MassTransit.Serialization;
using MassTransit.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace BigChange.MassTransit.AwsKeyManagementService
{
    public class EncryptedMessageSerializer :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/vnd.bigchange.masstransit+aes";
        public static readonly ContentType EncryptedContentType = new ContentType(ContentTypeHeaderValue);
        private readonly JsonSerializer _serializer;
        private readonly ICryptoStreamProvider _streamProvider;

        public EncryptedMessageSerializer(ICryptoStreamProvider streamProvider)
        {
            _streamProvider = streamProvider;
            _serializer = BsonMessageSerializer.Serializer;
        }

        ContentType IMessageSerializer.ContentType => EncryptedContentType;

        void IMessageSerializer.Serialize<T>(Stream stream, SendContext<T> context)
        {
            context.ContentType = EncryptedContentType;

            var envelope = new JsonMessageEnvelope(context, context.Message, TypeMetadataCache<T>.MessageTypeNames);

            using (var cryptoStream = _streamProvider.GetEncryptStream(stream, context))
            {
                using (var jsonWriter = new BsonDataWriter(cryptoStream))
                {
                    _serializer.Serialize(jsonWriter, envelope, typeof(MessageEnvelope));

                    jsonWriter.Flush();
                }
            }
        }
    }
}