using System;
using System.IO;
using Amazon.Runtime.SharedInterfaces;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
    public class GenerateDataKeyResultSerializable
    {
        public byte[] KeyCiphertext { get; set; }
        public byte[] KeyPlaintext { get; set; }

        public byte[] GetBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new BinaryWriter(ms))
                {
                    var keyCiphertextLengthBytes = BitConverter.GetBytes(KeyCiphertext.Length);
                    Array.Reverse(keyCiphertextLengthBytes);
                    var keyPlaintextLengthBytes = BitConverter.GetBytes(KeyPlaintext.Length);
                    Array.Reverse(keyPlaintextLengthBytes);

                    sw.Write(keyCiphertextLengthBytes);
                    sw.Write(KeyCiphertext);
                    sw.Write(keyPlaintextLengthBytes);
                    sw.Write(KeyPlaintext);

                    sw.Flush();
                }

                return ms.ToArray();
            }
        }

        public static GenerateDataKeyResult FromBytes(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                using (var sr = new BinaryReader(ms))
                {
                    var keyCiphertextLengthBytes = sr.ReadBytes(sizeof(int));
                    Array.Reverse(keyCiphertextLengthBytes);
                    var keyCiphertextLength = BitConverter.ToInt32(keyCiphertextLengthBytes, 0);

                    var obj = new GenerateDataKeyResultSerializable();
                    obj.KeyCiphertext = sr.ReadBytes(keyCiphertextLength);

                    var keyPlaintextLengthBytes = sr.ReadBytes(sizeof(int));
                    Array.Reverse(keyPlaintextLengthBytes);
                    var keyPlaintextLength = BitConverter.ToInt32(keyPlaintextLengthBytes, 0);

                    obj.KeyPlaintext = sr.ReadBytes(keyPlaintextLength);

                    return new GenerateDataKeyResult
                    {
                        KeyCiphertext = obj.KeyCiphertext,
                        KeyPlaintext = obj.KeyPlaintext
                    };
                }
            }
        }
    }
}