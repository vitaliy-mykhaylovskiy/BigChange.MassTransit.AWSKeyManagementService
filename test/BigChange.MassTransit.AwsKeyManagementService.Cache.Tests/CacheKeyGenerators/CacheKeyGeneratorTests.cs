using System;
using System.Collections.Generic;
using System.Linq;
using BigChange.MassTransit.AwsKeyManagementService.Cache.CacheKeyGenerators;
using NUnit.Framework;
using Shouldly;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache.Tests.CacheKeyGenerators
{
    [TestFixture]
    public class CacheKeyGeneratorTests
    {
        [Test]
        public void ShouldGenerateCacheKeyForKeydId()
        {
            var keyId = Guid.NewGuid().ToString();

            var encryptionContext = new Dictionary<string, string>()
            {
                { Guid.NewGuid().ToString(), Guid.NewGuid().ToString()},
                { Guid.NewGuid().ToString(), Guid.NewGuid().ToString()},
            };

            var sut = new CacheKeyGenerator();
            var key = sut.Generate(new DataKeyIdentifier(keyId, encryptionContext));

            key.ShouldBe(string.Join("", keyId, encryptionContext.Keys.ElementAt(0), encryptionContext.Values.ElementAt(0), encryptionContext.Keys.ElementAt(1), encryptionContext.Values.ElementAt(1)));
        }

        [Test]
        public void ShouldGenerateCacheKeyForCiphertextBlob()
        {
            var ciphertextBlob = new byte[] {1, 2, 3, 4};

            var encryptionContext = new Dictionary<string, string>()
            {
                { Guid.NewGuid().ToString(), Guid.NewGuid().ToString()},
                { Guid.NewGuid().ToString(), Guid.NewGuid().ToString()},
            };

            var sut = new CacheKeyGenerator();
            var key = sut.Generate(new DecryptIdentifier(ciphertextBlob, encryptionContext));

            key.ShouldBe(string.Join("", Convert.ToBase64String(ciphertextBlob), encryptionContext.Keys.ElementAt(0), encryptionContext.Values.ElementAt(0), encryptionContext.Keys.ElementAt(1), encryptionContext.Values.ElementAt(1)));
        }
    }
}