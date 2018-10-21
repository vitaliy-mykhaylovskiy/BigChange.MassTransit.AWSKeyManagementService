using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.KeyManagementService;
using Amazon.Runtime.SharedInterfaces;

namespace BigChange.MassTransit.AwsKeyManagementService
{
    public class AmazonKeyManagementServiceWrapper : IKeyManagementService
    {   
        private IAmazonKeyManagementService _service;

        public AmazonKeyManagementServiceWrapper(IAmazonKeyManagementService service)
        {
            _service = service;
        }

        public byte[] Decrypt(byte[] ciphertextBlob, Dictionary<string, string> encryptionContext)
        {
            return _service.Decrypt(ciphertextBlob, encryptionContext);
        }

        public GenerateDataKeyResult GenerateDataKey(string keyID, Dictionary<string, string> encryptionContext, string keySpec)
        {
            return _service.GenerateDataKey(keyID, encryptionContext, keySpec);
        }
    }
}