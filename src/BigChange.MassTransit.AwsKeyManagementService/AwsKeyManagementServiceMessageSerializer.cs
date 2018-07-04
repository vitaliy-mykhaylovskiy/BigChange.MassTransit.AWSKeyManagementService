using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Security.Cryptography;
using Amazon.KeyManagementService;
using MassTransit;
using MassTransit.Serialization;
using MassTransit.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace BigChange.MassTransit.AwsKeyManagementService
{
	public class AwsKeyManagementServiceMessageSerializer :
		IMessageSerializer
	{
		private readonly IAmazonKeyManagementService _amazonKeyManagementService;
		private readonly string _keyId;
		public const string ContentTypeHeaderValue = "application/vnd.masstransit+aws.kms.aes";
		public const string DataKeyCiphertextHeader = "KmsDataKeyCiphertext";
		public static readonly ContentType AwsKmsEncryptedContentType = new ContentType(ContentTypeHeaderValue);
		readonly JsonSerializer _serializer;
		private readonly PaddingMode _paddingMode;

		public AwsKeyManagementServiceMessageSerializer(IAmazonKeyManagementService amazonKeyManagementService, string keyId, PaddingMode paddingMode = PaddingMode.PKCS7)
		{
			_amazonKeyManagementService = amazonKeyManagementService;
			_keyId = keyId;
			_paddingMode = paddingMode;
			_serializer = BsonMessageSerializer.Serializer;
		}

		ContentType IMessageSerializer.ContentType => AwsKmsEncryptedContentType;

		void IMessageSerializer.Serialize<T>(Stream stream, SendContext<T> context)
		{
			context.ContentType = AwsKmsEncryptedContentType;

			var dataKeyResponse =
				_amazonKeyManagementService.GenerateDataKey(_keyId, new Dictionary<string, string>(){{"message_id", context.MessageId?.ToString()}}, "AES_256");

			context.Headers.Set(AwsKeyManagementServiceMessageSerializer.DataKeyCiphertextHeader, Convert.ToBase64String(dataKeyResponse.KeyCiphertext));
			var key = dataKeyResponse.KeyPlaintext;

			var iv = GenerateIv();


			var envelope = new JsonMessageEnvelope(context, context.Message, TypeMetadataCache<T>.MessageTypeNames);
			
			stream.Write(iv, 0, iv.Length);

			var encryptor = CreateEncryptor(key, iv);


			using (var jsonWriter = new BsonDataWriter(new DisposingCryptoStream(stream, encryptor, CryptoStreamMode.Write)))
			{
				_serializer.Serialize(jsonWriter, envelope, typeof(MessageEnvelope));

				jsonWriter.Flush();
			}
		}

		private byte[] GenerateIv()
		{
			var aes = CreateAes();
			aes.GenerateIV();

			return aes.IV;
		}


		private ICryptoTransform CreateEncryptor(byte[] key, byte[] iv)
		{
			using (var provider = CreateAes())
			{
				return provider.CreateEncryptor(key, iv);
			}
		}

		private Aes CreateAes()
			=> new AesCryptoServiceProvider { Padding = _paddingMode };
	}
}
