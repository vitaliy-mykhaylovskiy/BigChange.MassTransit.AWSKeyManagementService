using System.Collections.Generic;
using MassTransit;

namespace BigChange.MassTransit.AwsKeyManagementService
{
    public class MessageEncryptionContextBuilder : IEncryptionContextBuilder
    {
        public Dictionary<string, string> BuildEncryptionContext(SendContext context)
        {
            return new Dictionary<string, string>
            {
                {"message_id", context.MessageId?.ToString()}
            };
        }

        public Dictionary<string, string> BuildEncryptionContext(ReceiveContext receiveContext)
        {
            return new Dictionary<string, string>
            {
                {"message_id", receiveContext.TransportHeaders.Get<string>("message_id")}
            };
        }
    }
}