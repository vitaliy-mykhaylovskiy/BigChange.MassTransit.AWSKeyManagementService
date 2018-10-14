using System;
using Amazon.KeyManagementService;
using GreenPipes;
using MassTransit;
using MassTransit.Serialization;

namespace BigChange.MassTransit.AwsKeyManagementService
{
    public class KmsSecureKeyProvider : ISecureKeyProvider
    {
        public const string DataKeyCiphertextHeader = "KmsDataKeyCiphertext";
        private readonly IEncryptionContextBuilder _encryptionContextBuilder;
        private readonly string _kmsKeyId;
        private readonly IKeyManagementService _keyManagementService;

        public KmsSecureKeyProvider(IKeyManagementService keyManagementService,
            IEncryptionContextBuilder encryptionContextBuilder, string kmsKeyId)
        {
            _keyManagementService = keyManagementService;
            _encryptionContextBuilder = encryptionContextBuilder;
            _kmsKeyId = kmsKeyId;
        }

        public byte[] GetKey(ReceiveContext receiveContext)
        {
            var dataKeyCiphertext = GetDataKeyCiphertext(receiveContext);

            var encryptionContext = _encryptionContextBuilder.BuildEncryptionContext(receiveContext);
            var key = _keyManagementService.Decrypt(dataKeyCiphertext, encryptionContext);

            return key;
        }

        public byte[] GetKey(SendContext sendContext)
        {
            var encryptionContext = _encryptionContextBuilder.BuildEncryptionContext(sendContext);

            var dataKeyResponse =
                _keyManagementService.GenerateDataKey(_kmsKeyId, encryptionContext, "AES_256");

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