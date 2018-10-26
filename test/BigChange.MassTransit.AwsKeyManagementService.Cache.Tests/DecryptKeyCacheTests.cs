using System;
using System.Collections.Generic;
using BigChange.MassTransit.AwsKeyManagementService.Cache.CacheKeyGenerators;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache.Tests
{
    [TestFixture]
    public class DecryptKeyCacheTests
    {
        private Mock<ICacheKeyGenerator> _cacheKeyGenerator;
        private Mock<IDistributedCache> _distributedCache;
        private Mock<IDistributedCacheEntryOptionsFactory> _distributedCacheEntryOptionsFactory;

        [SetUp]
        public void SetUp()
        {
            _cacheKeyGenerator = new Mock<ICacheKeyGenerator>();
            _distributedCache = new Mock<IDistributedCache>();
            _distributedCacheEntryOptionsFactory = new Mock<IDistributedCacheEntryOptionsFactory>();
        }

        [Test]
        public void ShouldGetValue()
        {
            var identifier = new DecryptIdentifier(Guid.NewGuid().ToByteArray(), new Dictionary<string, string>());

            var cacheKey = Guid.NewGuid().ToString();
            _cacheKeyGenerator.Setup(x =>
                    x.Generate(identifier))
                .Returns(cacheKey);

            var cacheValue = new byte[] { 1, 2, 3 };
            _distributedCache.Setup(x => x.Get(cacheKey))
                .Returns(cacheValue);
            
            var dataKeyCache = new DecryptKeyCache(_cacheKeyGenerator.Object, _distributedCache.Object, _distributedCacheEntryOptionsFactory.Object);

            var result = dataKeyCache.Get(identifier);

            result.ShouldBe(cacheValue);
        }

        [Test]
        public void ShouldGetNullValue()
        {
            var identifier = new DecryptIdentifier(Guid.NewGuid().ToByteArray(), new Dictionary<string, string>());

            var cacheKey = Guid.NewGuid().ToString();
            _cacheKeyGenerator.Setup(x =>
                    x.Generate(identifier))
                .Returns(cacheKey);

            _distributedCache.Setup(x => x.Get(cacheKey))
                .Returns<byte[]>(null);

            var dataKeyCache = new DecryptKeyCache(_cacheKeyGenerator.Object, _distributedCache.Object, _distributedCacheEntryOptionsFactory.Object);

            var result = dataKeyCache.Get(identifier);

            result.ShouldBeNull();
        }


        [Test]
        public void ShouldSetValue()
        {
            var identifier = new DecryptIdentifier(Guid.NewGuid().ToByteArray(), new Dictionary<string, string>());

            var cacheKey = Guid.NewGuid().ToString();
            _cacheKeyGenerator.Setup(x =>
                    x.Generate(identifier))
                .Returns(cacheKey);

            var cacheValue = new byte[] { 1, 2, 3 };

            var entryOptions = new DistributedCacheEntryOptions();
            _distributedCacheEntryOptionsFactory.Setup(x => x.Create(CacheItemType.Decrypt))
                .Returns(entryOptions);

            var dataKeyCache = new DecryptKeyCache(_cacheKeyGenerator.Object, _distributedCache.Object, _distributedCacheEntryOptionsFactory.Object);

            dataKeyCache.Set(identifier, cacheValue);

            _distributedCache.Verify(x => x.Set(cacheKey, cacheValue, entryOptions));
        }
    }
}
