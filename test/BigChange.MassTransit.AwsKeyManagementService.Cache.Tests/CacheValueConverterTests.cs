using System.Linq;
using Amazon.Runtime.SharedInterfaces;
using NUnit.Framework;

namespace BigChange.MassTransit.AwsKeyManagementService.Cache.Tests
{
	[TestFixture]
	public class CacheValueConverterTests
	{
		[Test]
		public void ShouldConvertToCacheValue()
		{
			var cacheValueConverter = new CacheValueConverter();
			var result = cacheValueConverter.Convert(new GenerateDataKeyResult()
			{
				KeyCiphertext = new byte[] {1, 2},
				KeyPlaintext = new byte[] {3, 4, 5}
			});

			var expected = new byte[]{ 0, 0, 0, 2, 1, 2, 0, 0, 0, 3, 3, 4, 5 };
			Assert.True(result.SequenceEqual(expected));
		}

		[Test]
		public void ShouldConvertFromCacheValue()
		{
			var cacheValueConverter = new CacheValueConverter();
			var cacheValue = new byte[] { 0, 0, 0, 2, 1, 2, 0, 0, 0, 3, 3, 4, 5 };
			var result = cacheValueConverter.Convert(cacheValue);

			var expected = new GenerateDataKeyResult()
			{
				KeyCiphertext = new byte[] {1, 2},
				KeyPlaintext = new byte[] {3, 4, 5}
			};

			Assert.True(result.KeyCiphertext.SequenceEqual(expected.KeyCiphertext));
			Assert.True(result.KeyPlaintext.SequenceEqual(expected.KeyPlaintext));
		}
	}
}
