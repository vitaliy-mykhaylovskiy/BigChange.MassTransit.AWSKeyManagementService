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
        private Mock<IKeyManagementService> _keyManagementService;
        private Dictionary<string, string> _encryptionContext;

        private KeyManagementServiceCache _keyManagementServiceCache;
        private Mock<IDecryptKeyCache> _decryptKeyCache;
        private Mock<IDataKeyCache> _dataKeyCache;

        [SetUp]
        public void SetUp()
        {
            _keyManagementService = new Mock<IKeyManagementService>();

            _decryptKeyCache = new Mock<IDecryptKeyCache>();
            _dataKeyCache = new Mock<IDataKeyCache>();
            _keyManagementServiceCache = new KeyManagementServiceCache(_keyManagementService.Object, _dataKeyCache.Object, _decryptKeyCache.Object);

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

            var dataKeyResult = new GenerateDataKeyResult()
            {
                KeyCiphertext = new byte[] {1, 2, 3},
                KeyPlaintext = new byte[] {4, 5, 6}
            };

            _dataKeyCache.Setup(x => x.Get(It.Is<DataKeyIdentifier>(identifier =>
                    identifier.KeyId == keyId && identifier.EncryptionContext == _encryptionContext)))
                .Returns(dataKeyResult);

            var result =
                _keyManagementServiceCache.GenerateDataKey(keyId, _encryptionContext, Guid.NewGuid().ToString());

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
            
            var expected = new GenerateDataKeyResult()
            {
                KeyCiphertext = new byte[] {1, 2, 3},
                KeyPlaintext = new byte[] {4, 5, 6}
            };
            
            _dataKeyCache.Setup(x => x.Get(It.Is<DataKeyIdentifier>(identifier =>
                    identifier.KeyId == keyId && identifier.EncryptionContext == _encryptionContext)))
                .Returns<GenerateDataKeyResult>(null);

            var keySpec = Guid.NewGuid().ToString();
            _keyManagementService.Setup(x => x.GenerateDataKey(keyId, _encryptionContext, keySpec))
                .Returns(expected);

            var result = _keyManagementServiceCache.GenerateDataKey(keyId, _encryptionContext, keySpec);

            _dataKeyCache.Verify(x => x.Set(It.Is<DataKeyIdentifier>(identifier =>
                identifier.KeyId == keyId && identifier.EncryptionContext == _encryptionContext), expected), Times.Once);

            result.KeyPlaintext.ShouldBe(expected.KeyPlaintext);
            result.KeyCiphertext.ShouldBe(expected.KeyCiphertext);
        }

        [Test]
        public void ShouldReturnDecryptionKeyFromKeyManagementServiceOnDecrypt()
        {
            var keyCiphertext = Guid.NewGuid().ToByteArray();

            _decryptKeyCache.Setup(x => x.Get(It.Is<DecryptIdentifier>(identifier => identifier.CiphertextBlob == keyCiphertext && identifier.EncryptionContext == _encryptionContext)))
                .Returns<byte[]>(null);

            var cacheValue = new byte[] {10, 11, 12};

            _keyManagementService.Setup(
                    x => x.Decrypt(keyCiphertext, _encryptionContext))
                .Returns(cacheValue);

            var result = _keyManagementServiceCache.Decrypt(keyCiphertext, _encryptionContext);

            _decryptKeyCache.Verify(x => x.Set(It.Is<DecryptIdentifier>(identifier => identifier.CiphertextBlob == keyCiphertext && identifier.EncryptionContext == _encryptionContext), cacheValue), Times.Once);

            result.ShouldBe(cacheValue);
        }

        [Test]
        public void ShouldReturnDecryptionKeyFromCacheOnDecrypt()
        {
            var keyCiphertext = Guid.NewGuid().ToByteArray();

            var cacheValue = new byte[] { 10, 11, 12 };
            _decryptKeyCache.Setup(x => x.Get(It.Is<DecryptIdentifier>(identifier => identifier.CiphertextBlob == keyCiphertext && identifier.EncryptionContext == _encryptionContext)))
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