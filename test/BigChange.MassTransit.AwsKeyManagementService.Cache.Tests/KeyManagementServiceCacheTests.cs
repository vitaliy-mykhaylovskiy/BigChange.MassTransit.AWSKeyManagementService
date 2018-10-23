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
    public class KeyManagementServiceCacheTests
    {
        private Mock<IDistributedCache> _cache;
        private Mock<IKeyManagementService> _keyManagementService;
        private Dictionary<string, string> _encryptionContext;
	    private Mock<ICacheKeyGenerator> _cacheKeyGenerator;
	    private Mock<ICacheValueConverter> _cacheValueConverter;
	    private KeyManagementServiceCache _keyManagementServiceCache;

	    [SetUp]
        public void SetUp()
        {
            _cache = new Mock<IDistributedCache>();
            _keyManagementService = new Mock<IKeyManagementService>();
	        _cacheKeyGenerator = new Mock<ICacheKeyGenerator>();
	        _cacheValueConverter = new Mock<ICacheValueConverter>();

	        _keyManagementServiceCache = new KeyManagementServiceCache(_keyManagementService.Object,
		        _cache.Object, _cacheKeyGenerator.Object, _cacheValueConverter.Object);

			var contextKey = Guid.NewGuid().ToString();
            var contextValue = Guid.NewGuid().ToString();
            _encryptionContext = new Dictionary<string, string>
            {
                {contextKey, contextValue}
            };
        }

        [Test]
        public void ShouldReturnResultFromCacheOnGenerateDataKey()
        {
			var keyId = Guid.NewGuid().ToString();
			var cacheKey = Guid.NewGuid().ToString();
	        _cacheKeyGenerator.Setup(x =>
			        x.Generate(keyId, _encryptionContext))
		        .Returns(cacheKey);

	        var cacheValue = new byte[] {10, 11, 12};
	        _cache.Setup(x => x.Get(cacheKey))
		        .Returns(cacheValue);
	        var dataKeyResult = new GenerateDataKeyResult()
	        {
				KeyCiphertext = new byte[] {1,2,3},
				KeyPlaintext = new byte[] {4,5,6}
	        };
	        _cacheValueConverter.Setup(x => x.Convert(cacheValue))
		        .Returns(dataKeyResult);

			var result = _keyManagementServiceCache.GenerateDataKey(keyId, _encryptionContext, Guid.NewGuid().ToString());

            _keyManagementService.Verify(
                x => x.GenerateDataKey(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<string>()), Times.Never);

            result.KeyPlaintext.ShouldBe(dataKeyResult.KeyPlaintext);
            result.KeyCiphertext.ShouldBe(dataKeyResult.KeyCiphertext);
        }

	    [Test]
	    public void ShouldReturnResultFromKeyManagementServiceOnGenerateDataKey()
	    {
		    var keyId = Guid.NewGuid().ToString();
		    var cacheKey = Guid.NewGuid().ToString();
		    _cacheKeyGenerator.Setup(x =>
				    x.Generate(keyId, _encryptionContext))
			    .Returns(cacheKey);

		    var cacheValue = new byte[] {1, 2, 3};

			var expected = new GenerateDataKeyResult()
		    {
			    KeyCiphertext = new byte[] { 1, 2, 3 },
			    KeyPlaintext = new byte[] { 4, 5, 6 }
		    };
		    _cacheValueConverter.Setup(x => x.Convert(expected))
			    .Returns(cacheValue);

		    _cache.Setup(x => x.Get(cacheKey))
			    .Returns<byte[]>(null);

			var keySpec = Guid.NewGuid().ToString();
		    _keyManagementService.Setup(x => x.GenerateDataKey(keyId, _encryptionContext, keySpec))
			    .Returns(expected);

			var result = _keyManagementServiceCache.GenerateDataKey(keyId, _encryptionContext, keySpec);

		    _cache.Verify(x => x.Set(cacheKey, cacheValue, It.IsAny<DistributedCacheEntryOptions>()), Times.Once);

		    result.KeyPlaintext.ShouldBe(expected.KeyPlaintext);
		    result.KeyCiphertext.ShouldBe(expected.KeyCiphertext);
	    }

		[Test]
        public void ShouldReturnDecryptionKeyFromKeyManagementServiceOnDecrypt()
        {
	        var keyCiphertext = Guid.NewGuid().ToByteArray();
	        
			var cacheKey = Guid.NewGuid().ToString();
	        _cacheKeyGenerator.Setup(x =>
			        x.Generate(keyCiphertext, _encryptionContext))
		        .Returns(cacheKey);

	        _cache.Setup(x => x.Get(cacheKey))
		        .Returns<byte[]>(null);

			var cacheValue = new byte[] { 10, 11, 12 };
	        
			_keyManagementService.Setup(
				x => x.Decrypt(keyCiphertext, _encryptionContext))
				.Returns(cacheValue);

			var result = _keyManagementServiceCache.Decrypt(keyCiphertext, _encryptionContext);

	        _cache.Verify(x => x.Set(cacheKey, cacheValue, It.IsAny<DistributedCacheEntryOptions>()), Times.Once);

			result.ShouldBe(cacheValue);
        }

	    [Test]
	    public void ShouldReturnDecryptionKeyFromCacheOnDecrypt()
	    {
		    var keyCiphertext = Guid.NewGuid().ToByteArray();

		    var cacheKey = Guid.NewGuid().ToString();
		    _cacheKeyGenerator.Setup(x =>
				    x.Generate(keyCiphertext, _encryptionContext))
			    .Returns(cacheKey);

		    var cacheValue = new byte[] { 10, 11, 12 };
		    _cache.Setup(x => x.Get(cacheKey))
			    .Returns(cacheValue);

		    var result = _keyManagementServiceCache.Decrypt(keyCiphertext, _encryptionContext);

		    _keyManagementService.Verify(
			    x => x.Decrypt(
				    It.IsAny<byte[]>(),
				    It.IsAny<Dictionary<string, string>>()), Times.Never);

		    result.ShouldBe(cacheValue);
	    }
	}
}