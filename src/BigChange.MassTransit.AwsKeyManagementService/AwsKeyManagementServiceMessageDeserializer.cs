using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using Amazon.KeyManagementService;
using GreenPipes;
using MassTransit;
using MassTransit.Serialization;
using MassTransit.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace BigChange.MassTransit.AwsKeyManagementService
{
	public class AwsKeyManagementServiceMessageDeserializer :
		IMessageDeserializer
	{
		readonly JsonSerializer _deserializer;
		private readonly IAmazonKeyManagementService _amazonKeyManagementService;
		private readonly PaddingMode _paddingMode;
		readonly IObjectTypeDeserializer _objectTypeDeserializer;

		public AwsKeyManagementServiceMessageDeserializer(JsonSerializer deserializer,
			IAmazonKeyManagementService amazonKeyManagementService,
			PaddingMode paddingMode = PaddingMode.PKCS7)
		{
			_deserializer = deserializer;
			_amazonKeyManagementService = amazonKeyManagementService;
			_paddingMode = paddingMode;

			_objectTypeDeserializer = new ObjectTypeDeserializer(_deserializer);
		}

		void IProbeSite.Probe(ProbeContext context)
		{
			var scope = context.CreateScope("aws-kms");

			scope.Add("contentType", ContentType.MediaType);
			context.Add("paddingMode", _paddingMode.ToString());
		}

		public ContentType ContentType => AwsKeyManagementServiceMessageSerializer.AwsKmsEncryptedContentType;

		public ConsumeContext Deserialize(ReceiveContext receiveContext)
		{
			try
			{
				using (var body = receiveContext.GetBodyStream())
				{
					var dataKeyCiphertext = GetDataKeyCiphertext(receiveContext);


					var key = _amazonKeyManagementService.Decrypt(dataKeyCiphertext, new Dictionary<string, string>() { { "message_id", receiveContext.TransportHeaders.Get<string>("message_id")} });

					var iv = new byte[16];
					body.Read(iv, 0, iv.Length);

					var encryptor = CreateDecryptor(key, iv);

					using (var jsonReader =
						new BsonDataReader(new DisposingCryptoStream(body, encryptor, CryptoStreamMode.Read)))
					{
						var messageEnvelope = _deserializer.Deserialize<MessageEnvelope>(jsonReader);
						return new JsonConsumeContext(_deserializer, _objectTypeDeserializer, receiveContext,
							messageEnvelope);
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

		private byte[] GetDataKeyCiphertext(ReceiveContext receiveContext)
		{
			var base64DataKey =
				receiveContext.TransportHeaders.TryGetHeader(
					AwsKeyManagementServiceMessageSerializer.DataKeyCiphertextHeader, out var keyIdObj)
					? keyIdObj.ToString()
					: throw new Exception(
						$"{nameof(AwsKeyManagementServiceMessageSerializer.DataKeyCiphertextHeader)} Header is required");

			return Convert.FromBase64String(base64DataKey);
		}

		ICryptoTransform CreateDecryptor(byte[] key, byte[] iv)
		{
			using (var provider = CreateAes())
			{
				return provider.CreateDecryptor(key, iv);
			}
		}
		
		Aes CreateAes()
			=> new AesCryptoServiceProvider {Padding = _paddingMode};
	}
}