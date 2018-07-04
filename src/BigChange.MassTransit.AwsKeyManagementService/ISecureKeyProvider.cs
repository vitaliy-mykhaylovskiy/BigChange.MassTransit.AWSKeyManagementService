using GreenPipes;
using MassTransit;

namespace BigChange.MassTransit.AwsKeyManagementService
{
	public interface ISecureKeyProvider : IProbeSite
	{
		byte[] GetKey(ReceiveContext receiveContext);

		byte[] GetKey(SendContext sendContext);
	}
}