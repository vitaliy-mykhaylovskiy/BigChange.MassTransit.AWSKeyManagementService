using System.IO;
using System.Security.Cryptography;
using GreenPipes;
using MassTransit;

namespace BigChange.MassTransit.AwsKeyManagementService
{
    public class AesCryptoStreamProvider : ICryptoStreamProvider
    {
        private readonly PaddingMode _paddingMode;
        private readonly ISecureKeyProvider _secureKeyProvider;

        public AesCryptoStreamProvider(ISecureKeyProvider secureKeyProvider,
            PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            _secureKeyProvider = secureKeyProvider;
            _paddingMode = paddingMode;
        }

        public Stream GetDecryptStream(Stream stream, ReceiveContext context)
        {
            var key = _secureKeyProvider.GetKey(context);

            var iv = new byte[16];
            stream.Read(iv, 0, iv.Length);

            var encryptor = CreateDecryptor(key, iv);

            return new DisposingCryptoStream(stream, encryptor, CryptoStreamMode.Read);
        }

        public Stream GetEncryptStream(Stream stream, SendContext context)
        {
            var key = _secureKeyProvider.GetKey(context);

            var iv = GenerateIv();

            stream.Write(iv, 0, iv.Length);
            var encryptor = CreateEncryptor(key, iv);

            return new DisposingCryptoStream(stream, encryptor, CryptoStreamMode.Write);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("aes");

            scope.Add("paddingMode", _paddingMode.ToString());

            _secureKeyProvider.Probe(scope);
        }

        private ICryptoTransform CreateDecryptor(byte[] key, byte[] iv)
        {
            using (var provider = CreateAes())
            {
                return provider.CreateDecryptor(key, iv);
            }
        }

        private byte[] GenerateIv()
        {
            var aes = CreateAes();
            aes.GenerateIV();

            return aes.IV;
        }

        private ICryptoTransform CreateEncryptor(byte[] key, byte[] iv)
        {
            using (var provider = CreateAes())
            {
                return provider.CreateEncryptor(key, iv);
            }
        }

        private Aes CreateAes()
        {
            return new AesCryptoServiceProvider {Padding = _paddingMode};
        }
    }
}