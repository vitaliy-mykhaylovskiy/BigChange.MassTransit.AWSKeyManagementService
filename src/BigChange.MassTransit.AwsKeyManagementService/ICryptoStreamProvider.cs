using System.IO;
using GreenPipes;
using MassTransit;

namespace BigChange.MassTransit.AwsKeyManagementService
{
	public interface ICryptoStreamProvider : IProbeSite
	{
		Stream GetDecryptStream(Stream stream, ReceiveContext context);

		Stream GetEncryptStream(Stream stream, SendContext context);
	}
}