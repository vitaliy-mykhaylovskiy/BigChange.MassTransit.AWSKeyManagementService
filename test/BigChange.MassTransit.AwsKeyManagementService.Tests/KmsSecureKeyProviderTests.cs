using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.KeyManagementService;
using Amazon.Runtime.SharedInterfaces;
using MassTransit;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace BigChange.MassTransit.AwsKeyManagementService.Tests
{
	[TestFixture]
    public class KmsSecureKeyProviderTests
    {
	    [Test]
	    public void ShouldReturnKeyAndSetHeaderOnSend()
	    {
		    var keyId = Guid.NewGuid().ToString();

		    var keyCiphertext = Guid.NewGuid().ToByteArray();
		    var key = new byte[]
		    {
			    31, 182, 254, 29, 98, 114, 85, 168, 176, 48, 113,
			    206, 198, 176, 181, 125, 106, 134, 98, 217, 113,
			    158, 88, 75, 118, 223, 117, 160, 224, 1, 47, 162
		    };

		    var encryptionContextBuilder = new Mock<IEncryptionContextBuilder>();
		    var encryptionContext = new Dictionary<string, string>();
		    encryptionContextBuilder.Setup(x => x.BuildEncryptionContext(It.IsAny<SendContext>()))
			    .Returns(encryptionContext);

		    var amazonKeyManagementService = new Mock<IAmazonKeyManagementService>();
		    amazonKeyManagementService.Setup(x =>
				    x.GenerateDataKey(keyId, encryptionContext, "AES_256"))
			    .Returns(new GenerateDataKeyResult { KeyCiphertext = keyCiphertext, KeyPlaintext = key });


		    var sendHeader = new Mock<SendHeaders>();
		    var sendContext = new Mock<SendContext>();
		    sendContext.Setup(x => x.Headers)
			    .Returns(sendHeader.Object);

		    var kmsSecureKeyProvider = new KmsSecureKeyProvider(amazonKeyManagementService.Object, encryptionContextBuilder.Object, keyId);
		    var bytes = kmsSecureKeyProvider.GetKey(sendContext.Object);

			bytes.ShouldBe(key);
		    sendHeader.Verify(x => x.Set(KmsSecureKeyProvider.DataKeyCiphertextHeader, Convert.ToBase64String(keyCiphertext)), Times.Once);
		}

	    [Test]
	    public void ShouldReturnKeyAndSetHeaderOnReceive()
	    {
		    var keyId = Guid.NewGuid().ToString();

		    var keyCiphertext = Guid.NewGuid().ToByteArray();
		    var key = new byte[]
		    {
			    31, 182, 254, 29, 98, 114, 85, 168, 176, 48, 113,
			    206, 198, 176, 181, 125, 106, 134, 98, 217, 113,
			    158, 88, 75, 118, 223, 117, 160, 224, 1, 47, 162
		    };

		    var encryptionContextBuilder = new Mock<IEncryptionContextBuilder>();
		    var encryptionContext = new Dictionary<string, string>();
		    encryptionContextBuilder.Setup(x => x.BuildEncryptionContext(It.IsAny<ReceiveContext>()))
			    .Returns(encryptionContext);

		    var amazonKeyManagementService = new Mock<IAmazonKeyManagementService>();
		    amazonKeyManagementService.Setup(x =>
				    x.Decrypt(It.Is<byte[]>(bytes1 => bytes1.SequenceEqual(keyCiphertext)), encryptionContext))
			    .Returns(key);

		    var headers = new Mock<Headers>();
		    object base64KeyCiphertext = Convert.ToBase64String(keyCiphertext);

			headers.Setup(x => x.TryGetHeader(KmsSecureKeyProvider.DataKeyCiphertextHeader, out base64KeyCiphertext))
			    .Returns(true);

		    var receiveContext = new Mock<ReceiveContext>();
		    receiveContext.Setup(x => x.TransportHeaders)
			    .Returns(headers.Object);

		    var kmsSecureKeyProvider = new KmsSecureKeyProvider(amazonKeyManagementService.Object, encryptionContextBuilder.Object, keyId);
		    var bytes = kmsSecureKeyProvider.GetKey(receiveContext.Object);

		    bytes.ShouldBe(key);
	    }
	}
}
