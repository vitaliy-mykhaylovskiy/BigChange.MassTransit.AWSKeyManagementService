using System;
using System.IO;
using Amazon.Runtime.SharedInterfaces;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
	public class CacheValueConverter : ICacheValueConverter
	{
		public GenerateDataKeyResult Convert(byte[] cacheValue)
		{
			using (var ms = new MemoryStream(cacheValue))
			{
				using (var sr = new BinaryReader(ms))
				{
					var keyCiphertextLengthBytes = sr.ReadBytes(sizeof(int));
					Array.Reverse(keyCiphertextLengthBytes);
					var keyCiphertextLength = BitConverter.ToInt32(keyCiphertextLengthBytes, 0);

					var obj = new GenerateDataKeyResult();
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
	
		public byte[] Convert(GenerateDataKeyResult result)
		{
			using (var ms = new MemoryStream())
			{
				using (var sw = new BinaryWriter(ms))
				{
					var keyCiphertextLengthBytes = BitConverter.GetBytes(result.KeyCiphertext.Length);
					Array.Reverse(keyCiphertextLengthBytes);
					var keyPlaintextLengthBytes = BitConverter.GetBytes(result.KeyPlaintext.Length);
					Array.Reverse(keyPlaintextLengthBytes);

					sw.Write(keyCiphertextLengthBytes);
					sw.Write(result.KeyCiphertext);
					sw.Write(keyPlaintextLengthBytes);
					sw.Write(result.KeyPlaintext);

					sw.Flush();
				}

				return ms.ToArray();
			}
		} 
	}
}