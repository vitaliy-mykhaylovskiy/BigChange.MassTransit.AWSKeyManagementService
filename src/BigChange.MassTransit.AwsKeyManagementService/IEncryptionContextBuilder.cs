using System.Collections.Generic;
using MassTransit;

namespace BigChange.MassTransit.AwsKeyManagementService
{
    public interface IEncryptionContextBuilder
    {
        Dictionary<string, string> BuildEncryptionContext(SendContext context);

        Dictionary<string, string> BuildEncryptionContext(ReceiveContext receiveContext);
    }
}