using System.Collections.Generic;
using MassTransit;

namespace BigChange.MassTransit.AwsKeyManagementService
{
    public class EmptyEncryptionContextBuilder : IEncryptionContextBuilder
    {
        private static readonly Dictionary<string, string> EmptyEncryptionContext
            = new Dictionary<string, string>()
            {
                { "SomeKey", "SomeData" }
            };

        public Dictionary<string, string> BuildEncryptionContext(SendContext context)
            => EmptyEncryptionContext;

        public Dictionary<string, string> BuildEncryptionContext(ReceiveContext receiveContext)
            => EmptyEncryptionContext;
    }
}