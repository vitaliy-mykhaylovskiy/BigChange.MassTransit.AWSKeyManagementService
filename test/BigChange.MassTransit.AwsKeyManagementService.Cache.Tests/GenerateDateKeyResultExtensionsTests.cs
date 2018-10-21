using System.IO;
using System.Text;
using Amazon.Runtime.SharedInterfaces;
using NUnit.Framework;
using Shouldly;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache.Tests
{
    [TestFixture]
    public class GenerateDateKeyResultExtensionsTests
    {
        [Test]
        public void ShouldConvertFromByteArray()
        {
            var keyCiphertext = "keyciphertext";
            var keyPlaintext = "keyplaintext";
            var sut = new GenerateDataKeyResult
            {
                KeyCiphertext = Encoding.UTF8.GetBytes(keyCiphertext),
                KeyPlaintext = Encoding.UTF8.GetBytes(keyPlaintext)
            };

            var result = sut.FromByteArray(sut.ToByteArray());
            Encoding.UTF8.GetString(result.KeyCiphertext).ShouldBe(keyCiphertext);
        }
    }
}