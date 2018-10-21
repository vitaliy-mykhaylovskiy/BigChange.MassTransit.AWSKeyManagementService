using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Amazon.Runtime.SharedInterfaces;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
    public static class GenerateDataKeyResultExtensions
    {
        public static Byte[] ToByteArray(this GenerateDataKeyResult result)
        {
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, new GenerateDataKeyResultSerializable
                {
                    KeyCiphertext = result.KeyCiphertext,
                    KeyPlaintext = result.KeyPlaintext
                });
                return ms.ToArray();
            }
        }

        public static GenerateDataKeyResult FromByteArray(this GenerateDataKeyResult result, Byte[] item)
        {
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream(item))
            {
                var serializable = (GenerateDataKeyResultSerializable)formatter.Deserialize(ms);
                return new GenerateDataKeyResult
                {
                    KeyCiphertext = serializable.KeyCiphertext,
                    KeyPlaintext = serializable.KeyPlaintext
                };
            }
        }
    }

    [Serializable]
    public class GenerateDataKeyResultSerializable 
    {
        public byte[] KeyCiphertext { get; set; }
        public byte[] KeyPlaintext { get; set; }
    }
}