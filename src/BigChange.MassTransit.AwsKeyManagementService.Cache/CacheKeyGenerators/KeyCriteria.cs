using System.Collections.Generic;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache.CacheKeyGenerators
{
    public class KeyCriteria
    {
        public string KeyId { get; set; }
        public Dictionary<string,string> Context { get; set; }
    }
}