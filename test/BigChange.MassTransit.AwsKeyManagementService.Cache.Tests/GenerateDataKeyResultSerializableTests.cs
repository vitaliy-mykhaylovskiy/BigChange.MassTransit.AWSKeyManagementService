using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache.Tests
{
    [TestFixture]
    public class GenerateDataKeyResultSerializableTests
    {
        [TestCaseSource(nameof(TestCases))]
        public void ShouldGetBytes(GenerateDataKeyResultSerializable expected)
        {
            var bytes = expected.GetBytes();
            var actual = GenerateDataKeyResultSerializable.FromBytes(bytes);

            Assert.True(expected.KeyCiphertext.SequenceEqual(actual.KeyCiphertext));
            Assert.True(expected.KeyPlaintext.SequenceEqual(actual.KeyPlaintext));
        }

        static IEnumerable<GenerateDataKeyResultSerializable> TestCases()
        {
            yield return new GenerateDataKeyResultSerializable
            {
                KeyCiphertext = new byte[] { 1, 2, 3, 4, 5 },
                KeyPlaintext = new byte[] { 6, 7, 8, 9, 10 }
            };

            yield return new GenerateDataKeyResultSerializable
            {
                KeyCiphertext = new byte[] { 1, 2, 3 },
                KeyPlaintext = new byte[] { 4, 5, 6, 7, 8, 9, 10 }
            };
        }
    }
}