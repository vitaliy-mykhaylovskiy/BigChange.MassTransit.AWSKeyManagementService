using System;
using System.Collections.Generic;
using Amazon.Runtime.SharedInterfaces;
using BigChange.MassTransit.AwsKeyManagementService.Cache.CacheKeyGenerators;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache.Tests
{
    [TestFixture]
    public class DataKeyCacheTests
    {
        private Mock<ICacheKeyGenerator> _cacheKeyGenerator;
        private Mock<ICacheValueConverter> _cacheValueConverter;
        private Mock<IDistributedCache> _distributedCache;
        private Mock<IDistributedCacheEntryOptionsFactory> _distributedCacheEntryOptionsFactory;

        [SetUp]
        public void SetUp()
        {
            _cacheKeyGenerator = new Mock<ICacheKeyGenerator>();
            _cacheValueConverter = new Mock<ICacheValueConverter>();
            _distributedCache = new Mock<IDistributedCache>();
            _distributedCacheEntryOptionsFactory = new Mock<IDistributedCacheEntryOptionsFactory>();
        }

        [Test]
        public void ShouldGetValue()
        {
            var identifier = new DataKeyIdentifier(Guid.NewGuid().ToString(), new Dictionary<string, string>());

            var cacheKey = Guid.NewGuid().ToString();
            _cacheKeyGenerator.Setup(x =>
                    x.Generate(identifier))
                .Returns(cacheKey);

            var cacheValue = new byte[]{1,2,3};
            _distributedCache.Setup(x => x.Get(cacheKey))
                .Returns(cacheValue);

            var expected = new GenerateDataKeyResult();
            _cacheValueConverter.Setup(x => x.Convert(cacheValue))
                .Returns(expected);

            var dataKeyCache = new DataKeyCache(_cacheKeyGenerator.Object, _distributedCache.Object, _cacheValueConverter.Object, _distributedCacheEntryOptionsFactory.Object);

            var result = dataKeyCache.Get(identifier);

            result.ShouldBe(expected);
        }

        [Test]
        public void ShouldGetNullValue()
        {
            var identifier = new DataKeyIdentifier(Guid.NewGuid().ToString(), new Dictionary<string, string>());

            var cacheKey = Guid.NewGuid().ToString();
            _cacheKeyGenerator.Setup(x =>
                    x.Generate(identifier))
                .Returns(cacheKey);

            _distributedCache.Setup(x => x.Get(cacheKey))
                .Returns<byte[]>(null);
            
            var dataKeyCache = new DataKeyCache(_cacheKeyGenerator.Object, _distributedCache.Object, _cacheValueConverter.Object, _distributedCacheEntryOptionsFactory.Object);

            var result = dataKeyCache.Get(identifier);

            result.ShouldBeNull();
        }


        [Test]
        public void ShouldSetValue()
        {
            var identifier = new DataKeyIdentifier(Guid.NewGuid().ToString(), new Dictionary<string, string>());

            var cacheKey = Guid.NewGuid().ToString();
            _cacheKeyGenerator.Setup(x =>
                    x.Generate(identifier))
                .Returns(cacheKey);

            var cacheValue = new byte[] { 1, 2, 3 };

            var item = new GenerateDataKeyResult();
            _cacheValueConverter.Setup(x => x.Convert(item))
                .Returns(cacheValue);

            var entryOptions = new DistributedCacheEntryOptions();
            _distributedCacheEntryOptionsFactory.Setup(x => x.Create(CacheItemType.DataKey))
                .Returns(entryOptions);

            var dataKeyCache = new DataKeyCache(_cacheKeyGenerator.Object, _distributedCache.Object, _cacheValueConverter.Object, _distributedCacheEntryOptionsFactory.Object);

            dataKeyCache.Set(identifier, item);

            _distributedCache.Verify(x=> x.Set(cacheKey, cacheValue, entryOptions));
        }
    }
}
