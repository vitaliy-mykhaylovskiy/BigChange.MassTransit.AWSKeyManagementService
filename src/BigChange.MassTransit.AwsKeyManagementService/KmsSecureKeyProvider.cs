using System;
using Amazon.KeyManagementService;
using GreenPipes;
using MassTransit;

namespace BigChange.MassTransit.AwsKeyManagementService
{
    public class KmsSecureKeyProvider : ISecureKeyProvider
    {
        public const string DataKeyCiphertextHeader = "KmsDataKeyCiphertext";
        private readonly IAmazonKeyManagementService _amazonKeyManagementService;
        private readonly IEncryptionContextBuilder _encryptionContextBuilder;
        private readonly string _kmsKeyId;

        public KmsSecureKeyProvider(IAmazonKeyManagementService amazonKeyManagementService,
            IEncryptionContextBuilder encryptionContextBuilder, string kmsKeyId)
        {
            _amazonKeyManagementService = amazonKeyManagementService;
            _encryptionContextBuilder = encryptionContextBuilder;
            _kmsKeyId = kmsKeyId;
        }

        public byte[] GetKey(ReceiveContext receiveContext)
        {
            var dataKeyCiphertext = GetDataKeyCiphertext(receiveContext);

            var encryptionContext = _encryptionContextBuilder.BuildEncryptionContext(receiveContext);
            var key = _amazonKeyManagementService.Decrypt(dataKeyCiphertext, encryptionContext);

            return key;
        }

        public byte[] GetKey(SendContext sendContext)
        {
            var encryptionContext = _encryptionContextBuilder.BuildEncryptionContext(sendContext);

            var dataKeyResponse =
                _amazonKeyManagementService.GenerateDataKey(_kmsKeyId, encryptionContext, "AES_256");

            sendContext.Headers.Set(DataKeyCiphertextHeader, Convert.ToBase64String(dataKeyResponse.KeyCiphertext));

            return dataKeyResponse.KeyPlaintext;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("kms");

            scope.Add("kmskeyid", _kmsKeyId);
        }

        private byte[] GetDataKeyCiphertext(ReceiveContext receiveContext)
        {
            var base64DataKey =
                receiveContext.TransportHeaders.TryGetHeader(
                    DataKeyCiphertextHeader, out var keyIdObj)
                    ? keyIdObj.ToString()
                    : throw new Exception(
                        $"{nameof(DataKeyCiphertextHeader)} Header is required");

            return Convert.FromBase64String(base64DataKey);
        }
    }
}