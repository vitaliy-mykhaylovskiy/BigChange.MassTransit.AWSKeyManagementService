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
            string kmsKeyId, TimeSpan ttl)
        {
            var distributedCacheEntryOptionsFactory = new AbsoluteExpirationRelativeToNowOptionsFactory(ttl);

            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(kmsKeyId,
                distributedCacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IReceiveEndpointConfigurator configurator,
            string kmsKeyId, TimeSpan ttl)
        {
            var distributedCacheEntryOptionsFactory = new AbsoluteExpirationRelativeToNowOptionsFactory(ttl);

            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(kmsKeyId, distributedCacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IBusFactoryConfigurator configurator,
            string kmsKeyId, IDistributedCacheEntryOptionsFactory distributedCacheEntryOptionsFactory)
        {
            var options = CreateDefaultMemoryDistributedCacheOptions();

            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(kmsKeyId, options, distributedCacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IReceiveEndpointConfigurator configurator,
            string kmsKeyId, IDistributedCacheEntryOptionsFactory distributedCacheEntryOptionsFactory)
        {
            var options = CreateDefaultMemoryDistributedCacheOptions();

            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(kmsKeyId, options, distributedCacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IBusFactoryConfigurator configurator,
            string kmsKeyId, IOptions<MemoryDistributedCacheOptions> options)
        {
            var cacheEntryOptionsFactory = CreateDefaultDistributedCacheEntryOptionsFactory();

            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(kmsKeyId, options, cacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IReceiveEndpointConfigurator configurator,
            string kmsKeyId, IOptions<MemoryDistributedCacheOptions> options)
        {
            var cacheEntryOptionsFactory = CreateDefaultDistributedCacheEntryOptionsFactory();

            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(kmsKeyId, options, cacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IBusFactoryConfigurator configurator,
            string kmsKeyId, IOptions<MemoryDistributedCacheOptions> options, IDistributedCacheEntryOptionsFactory distributedCacheEntryOptionsFactory)
        {
            var amazonKeyManagementServiceClient = CreateDefaultAmazonKeyManagementServiceClient();

            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(kmsKeyId, amazonKeyManagementServiceClient, options, distributedCacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IReceiveEndpointConfigurator configurator,
            string kmsKeyId, IOptions<MemoryDistributedCacheOptions> options, IDistributedCacheEntryOptionsFactory distributedCacheEntryOptionsFactory)
        {
            var amazonKeyManagementServiceClient = CreateDefaultAmazonKeyManagementServiceClient();

            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(kmsKeyId, amazonKeyManagementServiceClient, options, distributedCacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IBusFactoryConfigurator configurator,
            RegionEndpoint region, string kmsKeyId, TimeSpan ttl)
        {
            var distributedCacheEntryOptionsFactory = new AbsoluteExpirationRelativeToNowOptionsFactory(ttl);

            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(region, kmsKeyId, distributedCacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IReceiveEndpointConfigurator configurator,
            RegionEndpoint region, string kmsKeyId, TimeSpan ttl)
        {
            var distributedCacheEntryOptionsFactory = new AbsoluteExpirationRelativeToNowOptionsFactory(ttl);

            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(region, kmsKeyId, distributedCacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IBusFactoryConfigurator configurator,
            RegionEndpoint region, string kmsKeyId, IDistributedCacheEntryOptionsFactory cacheEntryOptionsFactory)
        {
            var options = CreateDefaultMemoryDistributedCacheOptions();

            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(region, kmsKeyId, options, cacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IReceiveEndpointConfigurator configurator,
            RegionEndpoint region, string kmsKeyId, IDistributedCacheEntryOptionsFactory cacheEntryOptionsFactory)
        {
            var options = CreateDefaultMemoryDistributedCacheOptions();

            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(region, kmsKeyId, options, cacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IBusFactoryConfigurator configurator,
            RegionEndpoint region, string kmsKeyId, IOptions<MemoryDistributedCacheOptions> options)
        {
            var cacheEntryOptionsFactory = CreateDefaultDistributedCacheEntryOptionsFactory();

            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(region, kmsKeyId, options, cacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IReceiveEndpointConfigurator configurator,
            RegionEndpoint region, string kmsKeyId, IOptions<MemoryDistributedCacheOptions> options)
        {
            var cacheEntryOptionsFactory = CreateDefaultDistributedCacheEntryOptionsFactory();

            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(region, kmsKeyId, options, cacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IBusFactoryConfigurator configurator,
            RegionEndpoint region, string kmsKeyId, IOptions<MemoryDistributedCacheOptions> options,
            IDistributedCacheEntryOptionsFactory distributedCacheEntryOptionsFactory)
        {
            var amazonKeyManagementServiceClient = CreateDefaultAmazonKeyManagementServiceClient(region);

            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(kmsKeyId, amazonKeyManagementServiceClient,
                options, distributedCacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IReceiveEndpointConfigurator configurator,
            RegionEndpoint region, string kmsKeyId, IOptions<MemoryDistributedCacheOptions> options,
            IDistributedCacheEntryOptionsFactory distributedCacheEntryOptionsFactory)
        {
            var amazonKeyManagementServiceClient = CreateDefaultAmazonKeyManagementServiceClient(region);

            configurator.UseAwsKeyManagementServiceSerializerWithMemoryCache(kmsKeyId, amazonKeyManagementServiceClient,
                options, distributedCacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IBusFactoryConfigurator configurator,
            string kmsKeyId, IAmazonKeyManagementService amazonKeyManagementService,
            IOptions<MemoryDistributedCacheOptions> options, IDistributedCacheEntryOptionsFactory distributedCacheEntryOptionsFactory)
        {
            configurator.UseAwsKeyManagementServiceSerializerWithCache(kmsKeyId, amazonKeyManagementService,
                new MemoryDistributedCache(options), distributedCacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithMemoryCache(
            this IReceiveEndpointConfigurator configurator,
            string kmsKeyId, IAmazonKeyManagementService amazonKeyManagementService,
            IOptions<MemoryDistributedCacheOptions> options, IDistributedCacheEntryOptionsFactory distributedCacheEntryOptionsFactory)
        {
            configurator.UseAwsKeyManagementServiceSerializerWithCache(kmsKeyId, amazonKeyManagementService,
                new MemoryDistributedCache(options), distributedCacheEntryOptionsFactory);
        }
        
        public static void UseAwsKeyManagementServiceSerializerWithCache(this IBusFactoryConfigurator configurator,
            string kmsKeyId, IDistributedCache distributedCache)
        {
            var amazonKeyManagementServiceClient = CreateDefaultAmazonKeyManagementServiceClient();

            configurator.UseAwsKeyManagementServiceSerializerWithCache(
                kmsKeyId, amazonKeyManagementServiceClient, distributedCache);
        }

        public static void UseAwsKeyManagementServiceSerializerWithCache(this IReceiveEndpointConfigurator configurator,
            string kmsKeyId, IDistributedCache distributedCache)
        {
            var amazonKeyManagementServiceClient = CreateDefaultAmazonKeyManagementServiceClient();

            configurator.UseAwsKeyManagementServiceSerializerWithCache(
                kmsKeyId, amazonKeyManagementServiceClient, distributedCache);
        }

        public static void UseAwsKeyManagementServiceSerializerWithCache(this IBusFactoryConfigurator configurator,
            RegionEndpoint region, string kmsKeyId, IDistributedCache distributedCache)
        {
            var amazonKeyManagementServiceClient = CreateDefaultAmazonKeyManagementServiceClient(region);

            configurator.UseAwsKeyManagementServiceSerializerWithCache(
                kmsKeyId, amazonKeyManagementServiceClient, distributedCache);
        }

        public static void UseAwsKeyManagementServiceSerializerWithCache(this IReceiveEndpointConfigurator configurator,
            RegionEndpoint region, string kmsKeyId, IDistributedCache distributedCache)
        {
            var amazonKeyManagementServiceClient = CreateDefaultAmazonKeyManagementServiceClient(region);

            configurator.UseAwsKeyManagementServiceSerializerWithCache(
                kmsKeyId, amazonKeyManagementServiceClient, distributedCache);
        }

        public static void UseAwsKeyManagementServiceSerializerWithCache(this IBusFactoryConfigurator configurator,
            string kmsKeyId, IAmazonKeyManagementService amazonKeyManagementService, IDistributedCache distributedCache)
        {
            var distributedCacheEntryOptionsFactory = CreateDefaultDistributedCacheEntryOptionsFactory();

            configurator.UseAwsKeyManagementServiceSerializerWithCache(kmsKeyId, amazonKeyManagementService, distributedCache, distributedCacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithCache(this IReceiveEndpointConfigurator configurator,
            string kmsKeyId, IAmazonKeyManagementService amazonKeyManagementService, IDistributedCache distributedCache)
        {
            var distributedCacheEntryOptionsFactory = CreateDefaultDistributedCacheEntryOptionsFactory();

            configurator.UseAwsKeyManagementServiceSerializerWithCache(kmsKeyId, amazonKeyManagementService, distributedCache, distributedCacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithCache(this IBusFactoryConfigurator configurator,
            string kmsKeyId, IAmazonKeyManagementService amazonKeyManagementService, IDistributedCache distributedCache,
            IDistributedCacheEntryOptionsFactory distributedCacheEntryOptionsFactory)
        {
            var amazonKeyManagementServiceWrapper = new AmazonKeyManagementServiceWrapper(amazonKeyManagementService);

            var emptyEncryptionContextBuilder = CreateDefaultEncryptionContextBuilder();

            var cacheKeyGenerator = CreateDefaultCacheKeyGenerator();

            var cacheValueConverter = CreateDefaultCacheValueConverter();
            
            configurator.UseAwsKeyManagementServiceSerializerWithCache(amazonKeyManagementServiceWrapper,
                emptyEncryptionContextBuilder,
                kmsKeyId, distributedCache, cacheKeyGenerator, cacheValueConverter, distributedCacheEntryOptionsFactory);
        }

        public static void UseAwsKeyManagementServiceSerializerWithCache(this IReceiveEndpointConfigurator configurator,
            string kmsKeyId, IAmazonKeyManagementService amazonKeyManagementService, IDistributedCache distributedCache,
            IDistributedCacheEntryOptionsFactory distributedCacheEntryOptionsFactory)
        {
            var amazonKeyManagementServiceWrapper = new AmazonKeyManagementServiceWrapper(amazonKeyManagementService);

            var emptyEncryptionContextBuilder = CreateDefaultEncryptionContextBuilder();

            var cacheKeyGenerator = CreateDefaultCacheKeyGenerator();

            var cacheValueConverter = CreateDefaultCacheValueConverter();

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

            configurator.UseAwsKeyManagementServiceSerializerWithCache(amazonKeyManagementService, encryptionContextBuilder,
                kmsKeyId, dataKeyCache, decryptKeyCache);
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

            configurator.UseAwsKeyManagementServiceSerializerWithCache(amazonKeyManagementService, encryptionContextBuilder,
                kmsKeyId, dataKeyCache, decryptKeyCache);
        }


        public static void UseAwsKeyManagementServiceSerializerWithCache(this IBusFactoryConfigurator configurator,
            IKeyManagementService amazonKeyManagementService,
            IEncryptionContextBuilder encryptionContextBuilder,
            string kmsKeyId, IDataKeyCache dataKeyCache, IDecryptKeyCache decryptKeyCache)
        {
            var keyManagementServiceCache =
                new KeyManagementServiceCache(amazonKeyManagementService, dataKeyCache, decryptKeyCache);

            configurator.UseAwsKeyManagementServiceSerializer(keyManagementServiceCache, encryptionContextBuilder,
                kmsKeyId);
        }

        public static void UseAwsKeyManagementServiceSerializerWithCache(this IReceiveEndpointConfigurator configurator,
            IKeyManagementService amazonKeyManagementService,
            IEncryptionContextBuilder encryptionContextBuilder,
            string kmsKeyId, IDataKeyCache dataKeyCache, IDecryptKeyCache decryptKeyCache)
        {
            var keyManagementServiceCache =
                new KeyManagementServiceCache(amazonKeyManagementService, dataKeyCache, decryptKeyCache);

            configurator.UseAwsKeyManagementServiceSerializer(keyManagementServiceCache, encryptionContextBuilder,
                kmsKeyId);
        }

        private static IDistributedCacheEntryOptionsFactory CreateDefaultDistributedCacheEntryOptionsFactory()
        {
            return new AbsoluteExpirationRelativeToNowOptionsFactory(TimeSpan.FromMinutes(10));
        }

        private static IOptions<MemoryDistributedCacheOptions> CreateDefaultMemoryDistributedCacheOptions()
        {
            return Options.Create(new MemoryDistributedCacheOptions());
        }

        private static ICacheValueConverter CreateDefaultCacheValueConverter()
        {
            return new CacheValueConverter();
        }

        private static ICacheKeyGenerator CreateDefaultCacheKeyGenerator()
        {
            return new CacheKeyGenerator();
        }

        private static IEncryptionContextBuilder CreateDefaultEncryptionContextBuilder()
        {
            return new EmptyEncryptionContextBuilder();
        }

        private static IAmazonKeyManagementService CreateDefaultAmazonKeyManagementServiceClient(RegionEndpoint region)
        {
            return new AmazonKeyManagementServiceClient(region);
        }

        private static IAmazonKeyManagementService CreateDefaultAmazonKeyManagementServiceClient()
        {
            return new AmazonKeyManagementServiceClient();
        }
    }
}