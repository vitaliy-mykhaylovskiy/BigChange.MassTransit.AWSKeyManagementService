using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Runtime.SharedInterfaces;

namespace BigChange.MassTransit.AwsKeyManagementService
{
    public interface IKeyManagementService
    {
        byte[] Decrypt(byte[] ciphertextBlob, Dictionary<string, string> encryptionContext);

        GenerateDataKeyResult GenerateDataKey(string keyID, Dictionary<string, string> encryptionContext, string keySpec);
    }
}