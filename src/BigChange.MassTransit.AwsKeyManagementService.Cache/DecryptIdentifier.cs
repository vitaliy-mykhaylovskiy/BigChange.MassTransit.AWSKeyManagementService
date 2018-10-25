using System.Collections.Generic;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
    public class DecryptIdentifier
    {
        public DecryptIdentifier(byte[] ciphertextBlob, IReadOnlyDictionary<string, string> encryptionContext)
        {
            CiphertextBlob = ciphertextBlob;
            EncryptionContext = encryptionContext;
        }

        public byte[] CiphertextBlob { get; }

        public IReadOnlyDictionary<string, string> EncryptionContext { get; }
    }
}