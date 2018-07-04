using System;
using System.Net.Mime;
using System.Runtime.Serialization;
using GreenPipes;
using MassTransit;
using MassTransit.Serialization;
using MassTransit.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace BigChange.MassTransit.AwsKeyManagementService
{
	public class EncryptedMessageDeserializer :
		IMessageDeserializer
	{
		private readonly JsonSerializer _deserializer;
		private readonly ICryptoStreamProvider _cryptoStreamProvider;
		private readonly IObjectTypeDeserializer _objectTypeDeserializer;

		public EncryptedMessageDeserializer(JsonSerializer deserializer, ICryptoStreamProvider cryptoStreamProvider)
		{
			_deserializer = deserializer;
			_cryptoStreamProvider = cryptoStreamProvider;
			_objectTypeDeserializer = new ObjectTypeDeserializer(_deserializer);
		}

		void IProbeSite.Probe(ProbeContext context)
		{
			ProbeContext scope = context.CreateScope("encrypted");
			scope.Add("contentType", EncryptedMessageSerializer.EncryptedContentType.MediaType);
			_cryptoStreamProvider.Probe(scope);
		}

		public ContentType ContentType => EncryptedMessageSerializer.EncryptedContentType;

		public ConsumeContext Deserialize(ReceiveContext receiveContext)
		{
			try
			{
				using (var body = receiveContext.GetBodyStream())
				{
					using (var disposingCryptoStream = _cryptoStreamProvider.GetDecryptStream(body, receiveContext))
					{
						using (var jsonReader = new BsonDataReader(disposingCryptoStream))
						{
							var messageEnvelope = _deserializer.Deserialize<MessageEnvelope>(jsonReader);

							return new JsonConsumeContext(_deserializer, _objectTypeDeserializer, receiveContext,
								messageEnvelope);
						}
					}
				}
			}
			catch (JsonSerializationException ex)
			{
				throw new SerializationException(
					"A JSON serialization exception occurred while deserializing the message envelope", ex);
			}
			catch (SerializationException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new SerializationException("An exception occurred while deserializing the message envelope", ex);
			}
		}

	}
}