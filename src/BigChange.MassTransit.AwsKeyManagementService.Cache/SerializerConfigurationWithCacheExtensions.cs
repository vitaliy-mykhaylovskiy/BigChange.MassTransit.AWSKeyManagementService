using System;
using Amazon;
using Amazon.KeyManagementService;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using BigChange.MassTransit.AwsKeyManagementService.Cache.CacheKeyGenerators;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache
{
    public static class SerializerConfigurationWithCacheExtensions
    {
        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IBusFactoryConfigurator configurator,
            string kmsKeyId, IOptions<MemoryDistributedCacheOptions> options)
        {
            var amazonKeyManagementServiceClient = new AmazonKeyManagementServiceClient();
            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(kmsKeyId, amazonKeyManagementServiceClient,
                options);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IReceiveEndpointConfigurator configurator,
            string kmsKeyId, IOptions<MemoryDistributedCacheOptions> options)
        {
            var amazonKeyManagementServiceClient = new AmazonKeyManagementServiceClient();
            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(kmsKeyId, amazonKeyManagementServiceClient,
                options);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IBusFactoryConfigurator configurator,
            RegionEndpoint region, string kmsKeyId, IOptions<MemoryDistributedCacheOptions> options)
        {
            var amazonKeyManagementServiceClient = new AmazonKeyManagementServiceClient(region);
            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(kmsKeyId, amazonKeyManagementServiceClient,
                options);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IReceiveEndpointConfigurator configurator,
            RegionEndpoint region, string kmsKeyId, IOptions<MemoryDistributedCacheOptions> options)
        {
            var amazonKeyManagementServiceClient = new AmazonKeyManagementServiceClient(region);
            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(kmsKeyId, amazonKeyManagementServiceClient,
                options);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IBusFactoryConfigurator configurator,
            string kmsKeyId, IAmazonKeyManagementService amazonKeyManagementService,
            IOptions<MemoryDistributedCacheOptions> options)
        {
            configurator.UseAwsKeyManagementServiceSerializerWithCache(kmsKeyId, amazonKeyManagementService,
                new MemoryDistributedCache(options));
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IReceiveEndpointConfigurator configurator,
            string kmsKeyId, IAmazonKeyManagementService amazonKeyManagementService,
            IOptions<MemoryDistributedCacheOptions> options)
        {
            configurator.UseAwsKeyManagementServiceSerializerWithCache(kmsKeyId, amazonKeyManagementService,
                new MemoryDistributedCache(options));
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
            RegionEndpoint region, string kmsKeyId, IDistributedCache distributedCache)
        {
            var amazonKeyManagementServiceClient = new AmazonKeyManagementServiceClient(region);

            configurator.UseAwsKeyManagementServiceSerializerWithCache(
                kmsKeyId, amazonKeyManagementServiceClient, distributedCache);
        }

        public static void UseAwsKeyManagementServiceSerializerWithCache(this IReceiveEndpointConfigurator configurator,
            RegionEndpoint region, string kmsKeyId, IDistributedCache distributedCache)
        {
            var amazonKeyManagementServiceClient = new AmazonKeyManagementServiceClient(region);

            configurator.UseAwsKeyManagementServiceSerializerWithCache(
                kmsKeyId, amazonKeyManagementServiceClient, distributedCache);
        }

        public static void UseAwsKeyManagementServiceSerializerWithCache(this IBusFactoryConfigurator configurator,
            string kmsKeyId, IAmazonKeyManagementService amazonKeyManagementService, IDistributedCache distributedCache)
        {
            var amazonKeyManagementServiceWrapper = new AmazonKeyManagementServiceWrapper(amazonKeyManagementService);

            var emptyEncryptionContextBuilder = new EmptyEncryptionContextBuilder();

            var cacheKeyGenerator = new CacheKeyGenerator();

            var cacheValueConverter = new CacheValueConverter();

            var distributedCacheEntryOptionsFactory = new AbsoluteExpirationRelativeToNowOptionsFactory(TimeSpan.FromMinutes(10));

            configurator.UseAwsKeyManagementServiceSerializerWithCache(amazonKeyManagementServiceWrapper,
                emptyEncryptionContextBuilder,
                kmsKeyId, distributedCache, cacheKeyGenerator, cacheValueConverter, distributedCacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithCache(this IReceiveEndpointConfigurator configurator,
            string kmsKeyId, IAmazonKeyManagementService amazonKeyManagementService, IDistributedCache distributedCache)
        {
            var amazonKeyManagementServiceWrapper = new AmazonKeyManagementServiceWrapper(amazonKeyManagementService);

            var emptyEncryptionContextBuilder = new EmptyEncryptionContextBuilder();

            var cacheKeyGenerator = new CacheKeyGenerator();

            var cacheValueConverter = new CacheValueConverter();

            var distributedCacheEntryOptionsFactory = new AbsoluteExpirationRelativeToNowOptionsFactory(TimeSpan.FromMinutes(10));

            configurator.UseAwsKeyManagementServiceSerializerWithCache(amazonKeyManagementServiceWrapper,
                emptyEncryptionContextBuilder,
                kmsKeyId, distributedCache, cacheKeyGenerator, cacheValueConverter, distributedCacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithCache(this IBusFactoryConfigurator configurator,
            IKeyManagementService amazonKeyManagementService,
            IEncryptionContextBuilder encryptionContextBuilder,
            string kmsKeyId,
            IDistributedCache distributedCache,
            ICacheKeyGenerator cacheKeyGenerator,
            ICacheValueConverter cacheValueConverter,
            IDistributedCacheEntryOptionsFactory distributedCacheEntryOptionsFactory)
        {
            var dataKeyCache = new DataKeyCache(cacheKeyGenerator, distributedCache, cacheValueConverter, distributedCacheEntryOptionsFactory);
            var decryptKeyCache = new DecryptKeyCache(cacheKeyGenerator, distributedCache, distributedCacheEntryOptionsFactory);

            var keyManagementServiceCache =
                new KeyManagementServiceCache(amazonKeyManagementService, dataKeyCache, decryptKeyCache);

            configurator.UseAwsKeyManagementServiceSerializer(keyManagementServiceCache, encryptionContextBuilder,
                kmsKeyId);
        }

        public static void UseAwsKeyManagementServiceSerializerWithCache(this IReceiveEndpointConfigurator configurator,
            IKeyManagementService amazonKeyManagementService,
            IEncryptionContextBuilder encryptionContextBuilder,
            string kmsKeyId,
            IDistributedCache distributedCache,
            ICacheKeyGenerator cacheKeyGenerator,
            ICacheValueConverter cacheValueConverter,
            IDistributedCacheEntryOptionsFactory distributedCacheEntryOptionsFactory)
        {
            var dataKeyCache = new DataKeyCache(cacheKeyGenerator, distributedCache, cacheValueConverter, distributedCacheEntryOptionsFactory);
            var decryptKeyCache = new DecryptKeyCache(cacheKeyGenerator, distributedCache, distributedCacheEntryOptionsFactory);

            var keyManagementServiceCache =
                new KeyManagementServiceCache(amazonKeyManagementService, dataKeyCache, decryptKeyCache);

            configurator.UseAwsKeyManagementServiceSerializer(keyManagementServiceCache, encryptionContextBuilder,
                kmsKeyId);
        }
    }
}