﻿using Amazon.KeyManagementService;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using BigChange.MassTransit.AwsKeyManagementService.Cache.CacheKeyGenerators;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
    public static class SerializerConfigurationWithCacheExtensions
    {
	    public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(this IBusFactoryConfigurator configurator,
		    string kmsKeyId, IOptions<MemoryDistributedCacheOptions> options)
	    {
		    var amazonKeyManagementServiceClient = new AmazonKeyManagementServiceClient();
		    configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(kmsKeyId, amazonKeyManagementServiceClient, options);
		}

		public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(this IReceiveEndpointConfigurator configurator,
		    string kmsKeyId, IOptions<MemoryDistributedCacheOptions> options)
	    {
		    var amazonKeyManagementServiceClient = new AmazonKeyManagementServiceClient();
		    configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(kmsKeyId, amazonKeyManagementServiceClient, options);
		}

	    public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(this IBusFactoryConfigurator configurator,
		    string kmsKeyId, IAmazonKeyManagementService amazonKeyManagementService, IOptions<MemoryDistributedCacheOptions> options)
	    {
		    configurator.UseAwsKeyManagementServiceSerializerWithCache(kmsKeyId, amazonKeyManagementService, new MemoryDistributedCache(options));
	    }

	    public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(this IReceiveEndpointConfigurator configurator,
		    string kmsKeyId, IAmazonKeyManagementService amazonKeyManagementService, IOptions<MemoryDistributedCacheOptions> options)
	    {
		    configurator.UseAwsKeyManagementServiceSerializerWithCache(kmsKeyId, amazonKeyManagementService, new MemoryDistributedCache(options));
	    }

		public static void UseAwsKeyManagementServiceSerializerWithCache(this IBusFactoryConfigurator configurator,
		    string kmsKeyId, IDistributedCache distributedCache)
	    {
		    var amazonKeyManagementServiceClient = new AmazonKeyManagementServiceClient();

		    configurator.UseAwsKeyManagementServiceSerializerWithCache(
			    kmsKeyId, amazonKeyManagementServiceClient, distributedCache);
		}

	    public static void UseAwsKeyManagementServiceSerializerWithCache(this IReceiveEndpointConfigurator configurator,
		    string kmsKeyId, IDistributedCache distributedCache)
	    {
		    var amazonKeyManagementServiceClient = new AmazonKeyManagementServiceClient();

		    configurator.UseAwsKeyManagementServiceSerializerWithCache(
			    kmsKeyId, amazonKeyManagementServiceClient, distributedCache);
	    }

	    public static void UseAwsKeyManagementServiceSerializerWithCache(this IBusFactoryConfigurator configurator,
		    string kmsKeyId, IAmazonKeyManagementService amazonKeyManagementService, IDistributedCache distributedCache)
	    {
		    var amazonKeyManagementServiceWrapper = new AmazonKeyManagementServiceWrapper(amazonKeyManagementService);

		    var emptyEncryptionContextBuilder = new EmptyEncryptionContextBuilder();

		    var cacheKeyGenerator = new CacheKeyGenerator();

		    configurator.UseAwsKeyManagementServiceSerializerWithCache(amazonKeyManagementServiceWrapper, emptyEncryptionContextBuilder,
			    kmsKeyId, distributedCache, cacheKeyGenerator);
	    }

	    public static void UseAwsKeyManagementServiceSerializerWithCache(this IReceiveEndpointConfigurator configurator,
		    string kmsKeyId, IAmazonKeyManagementService amazonKeyManagementService, IDistributedCache distributedCache)
	    {
		    var amazonKeyManagementServiceWrapper = new AmazonKeyManagementServiceWrapper(amazonKeyManagementService);

		    var emptyEncryptionContextBuilder = new EmptyEncryptionContextBuilder();

		    var cacheKeyGenerator = new CacheKeyGenerator();

		    configurator.UseAwsKeyManagementServiceSerializerWithCache(amazonKeyManagementServiceWrapper, emptyEncryptionContextBuilder,
			    kmsKeyId, distributedCache, cacheKeyGenerator);
	    }

		public static void UseAwsKeyManagementServiceSerializerWithCache(this IBusFactoryConfigurator configurator,
            IKeyManagementService amazonKeyManagementService, 
            IEncryptionContextBuilder encryptionContextBuilder,
            string kmsKeyId,
            IDistributedCache distributedCache,
            ICacheKeyGenerator cacheKeyGenerator)
        {
	        var keyManagementServiceCache =
		        new KeyManagementServiceCache(amazonKeyManagementService, distributedCache, cacheKeyGenerator);

	        configurator.UseAwsKeyManagementServiceSerializer(keyManagementServiceCache, encryptionContextBuilder,
		        kmsKeyId);
		}

        public static void UseAwsKeyManagementServiceSerializerWithCache(this IReceiveEndpointConfigurator configurator,
            IKeyManagementService amazonKeyManagementService, 
            IEncryptionContextBuilder encryptionContextBuilder,
            string kmsKeyId,
            IDistributedCache distributedCache,
            ICacheKeyGenerator cacheKeyGenerator)
        {
	        var keyManagementServiceCache =
		        new KeyManagementServiceCache(amazonKeyManagementService, distributedCache, cacheKeyGenerator);

	        configurator.UseAwsKeyManagementServiceSerializer(keyManagementServiceCache, encryptionContextBuilder,
		        kmsKeyId);
        }
    }
}