using System;
using System.IO;
using Amazon.Runtime.SharedInterfaces;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
    [Serializable]
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
                    var KeyCiphertextLengthBytes = BitConverter.GetBytes(KeyCiphertext.Length);
                    Array.Reverse(KeyCiphertextLengthBytes);
                    var KeyPlaintextLengthBytes = BitConverter.GetBytes(KeyPlaintext.Length);
                    Array.Reverse(KeyPlaintextLengthBytes);

                    sw.Write(KeyCiphertextLengthBytes);
                    sw.Write(KeyCiphertext);
                    sw.Write(KeyPlaintextLengthBytes);
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
                    var KeyCiphertextLengthBytes = sr.ReadBytes(sizeof(int));
                    Array.Reverse(KeyCiphertextLengthBytes);
                    var KeyCiphertextLength = BitConverter.ToInt32(KeyCiphertextLengthBytes, 0);

                    var obj = new GenerateDataKeyResultSerializable();
                    obj.KeyCiphertext = sr.ReadBytes(KeyCiphertextLength);

                    var KeyPlaintextLengthBytes = sr.ReadBytes(sizeof(int));
                    Array.Reverse(KeyPlaintextLengthBytes);
                    var KeyPlaintextLength = BitConverter.ToInt32(KeyPlaintextLengthBytes, 0);

                    obj.KeyPlaintext = sr.ReadBytes(KeyPlaintextLength);

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