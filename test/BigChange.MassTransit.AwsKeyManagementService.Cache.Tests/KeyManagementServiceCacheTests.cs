using System;
using System.Collections.Generic;
using Amazon.Runtime.SharedInterfaces;
using BigChange.MassTransit.AwsKeyManagementService.Cache.CacheKeyGenerators;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache.Tests
{
    [TestFixture]
    public class KeyManagementServiceCacheTests
    {
        private Mock<IDistributedCache> _mockCache;
        private Mock<IKeyManagementService> _mockKeyManagementService;
        private string _keyId;
        private byte[] _keyCiphertext;
        private string _contextKey;
        private string _contextValue;
        private Dictionary<string, string> _encryptionContext;
        private byte[] _key;

        [SetUp]
        public void SetUp()
        {
            _mockCache = new Mock<IDistributedCache>();
            _mockKeyManagementService = new Mock<IKeyManagementService>();
            _keyId = Guid.NewGuid().ToString();
            _keyCiphertext = Guid.NewGuid().ToByteArray();
            _contextKey = Guid.NewGuid().ToString();
            _contextValue = Guid.NewGuid().ToString();
            _encryptionContext = new Dictionary<string, string>
            {
                {_contextKey, _contextValue}
            };
            _key = new byte[]
            {
                31, 182, 254, 29, 98, 114, 85, 168, 176, 48, 113,
                206, 198, 176, 181, 125, 106, 134, 98, 217, 113,
                158, 88, 75, 118, 223, 117, 160, 224, 1, 47, 162
            };
        }

        [Test]
        public void ShouldReturnResultFromCacheOnGenerateDataKey()
        {
            var cachedItem = new GenerateDataKeyResultSerializable
            {
                KeyCiphertext = _keyCiphertext,
                KeyPlaintext = _key
            };
            var mockCacheKeyGenerator = new Mock<ICacheKeyGenerator>();

            _mockCache.Setup(x => x.Get(It.IsAny<string>())).Returns(cachedItem.GetBytes());

            var result = new KeyManagementServiceCache(_mockKeyManagementService.Object, _mockCache.Object, mockCacheKeyGenerator.Object)
            .GenerateDataKey(_keyId, _encryptionContext, Guid.NewGuid().ToString());

            _mockKeyManagementService.Verify(
                x => x.GenerateDataKey(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<string>()), Times.Never);

            result.KeyPlaintext.ShouldBe(cachedItem.KeyPlaintext);
            result.KeyCiphertext.ShouldBe(cachedItem.KeyCiphertext);
        } 

        [Test]
        public void ShouldReturnKeyFromCacheOnDecrypt()
        {
            var cacheKeyGenerator = new Mock<ICacheKeyGenerator>();
            _mockCache.Setup(x => x.Get(It.IsAny<string>())).Returns(_key);
            
            var result = new KeyManagementServiceCache(_mockKeyManagementService.Object, _mockCache.Object, cacheKeyGenerator.Object)
                .Decrypt(_keyCiphertext, _encryptionContext);

            _mockKeyManagementService.Verify(
                x => x.GenerateDataKey(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<string>()), Times.Never);

            result.ShouldBe(_key);
        }
    }
}