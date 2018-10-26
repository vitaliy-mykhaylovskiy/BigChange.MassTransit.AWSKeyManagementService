using System.Collections.Generic;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
    public class DataKeyIdentifier
    {
        public string KeyId { get; }

        public Dictionary<string, string> EncryptionContext { get; }

        public DataKeyIdentifier(string keyId, Dictionary<string, string> encryptionContext)
        {
            KeyId = keyId;
            EncryptionContext = encryptionContext;
        }
    }
}