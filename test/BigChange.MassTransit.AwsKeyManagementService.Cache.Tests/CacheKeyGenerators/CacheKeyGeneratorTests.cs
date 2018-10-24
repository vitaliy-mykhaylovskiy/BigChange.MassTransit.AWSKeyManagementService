using System;
using System.Collections.Generic;
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
            var _expectedKeyId = "keyid";
            var _expectedDictionaryKey1 = "dickey1";
            var _expectedDictionaryValue1 = "valuekey1";
            var _expectedDictionaryKey2 = "dickey2";
            var _expectedDictionaryValue2 = "valuekey2";

            var key = new CacheKeyGenerator().Generate(_expectedKeyId,
                new Dictionary<string, string>()
                {
                    {_expectedDictionaryKey1, _expectedDictionaryValue1},
                    {_expectedDictionaryKey2, _expectedDictionaryValue2}
                }
            );

            key.ShouldBe(string.Join("",
                new[]
                {
                    _expectedKeyId, _expectedDictionaryKey1, _expectedDictionaryValue1, _expectedDictionaryKey2,
                    _expectedDictionaryValue2
                }));
        }

        [Test]
        public void ShouldGenerateCacheKeyForCiphertextBlob()
        {
            var ciphertextBlob = new byte[] {1, 2, 3, 4};

            var _expectedDictionaryKey1 = "dickey1";
            var _expectedDictionaryValue1 = "valuekey1";
            var _expectedDictionaryKey2 = "dickey2";
            var _expectedDictionaryValue2 = "valuekey2";

            var key = new CacheKeyGenerator().Generate(ciphertextBlob,
                new Dictionary<string, string>()
                {
                    {_expectedDictionaryKey1, _expectedDictionaryValue1},
                    {_expectedDictionaryKey2, _expectedDictionaryValue2}
                }
            );

            key.ShouldBe(string.Join("",
                new[]
                {
                    Convert.ToBase64String(ciphertextBlob), _expectedDictionaryKey1, _expectedDictionaryValue1,
                    _expectedDictionaryKey2, _expectedDictionaryValue2
                }));
        }
    }
}