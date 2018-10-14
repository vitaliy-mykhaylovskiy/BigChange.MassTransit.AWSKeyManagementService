using System;
using System.Collections.Generic;
using Amazon.Runtime.SharedInterfaces;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace BigChange.MassTransit.AwsKeyManagementService.Tests
{
    [TestFixture]
    public class KeyManagementServiceCacheTests
    {
        private MemoryCache _memoryCache;
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
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
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
            var expected = new GenerateDataKeyResult
            {
                KeyCiphertext = _keyCiphertext,
                KeyPlaintext = _key
            };

            _memoryCache.Set(string.Join("", new[] { _keyId, _contextKey, _contextValue }), expected);

            var result = new KeyManagementServiceCache(_mockKeyManagementService.Object, _memoryCache)
            .GenerateDataKey(_keyId, _encryptionContext, Guid.NewGuid().ToString());

            _mockKeyManagementService.Verify(
                x => x.GenerateDataKey(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<string>()), Times.Never);

            result.ShouldBe(expected);
        }

        [Test]
        public void ShouldReturnKeyFromCacheOnDecrypt()
        {
            _memoryCache.Set(string.Join("", new[]
            {
                System.Text.Encoding.UTF8.GetString(_keyCiphertext),
                _contextKey,
                _contextValue
            }), _key);

            var result = new KeyManagementServiceCache(_mockKeyManagementService.Object, _memoryCache)
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