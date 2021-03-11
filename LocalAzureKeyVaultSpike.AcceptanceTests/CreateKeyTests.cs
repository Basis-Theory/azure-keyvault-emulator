using System;
using System.Text;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using LocalAzureKeyVaultSpike.AcceptanceTests.Helpers;
using Xunit;

namespace LocalAzureKeyVaultSpike.AcceptanceTests
{
    public class CreateKeyTests
    {
        private readonly KeyClient _keyClient;
        private readonly UTF8Encoding _encoder = new UTF8Encoding();

        public CreateKeyTests()
        {
            _keyClient = new KeyClient(new Uri("https://localhost:5001/"), new LocalTokenCredential());
        }

        [Fact]
        public void ShouldBeAbleToCreateAKeyToEncryptAndDecryptData()
        {
            const string plaintextValue = "super-secret-stuff";
            var result = _keyClient.CreateRsaKey(new CreateRsaKeyOptions("foo-rsa")
            {
                Enabled = true,
                ExpiresOn = DateTimeOffset.UtcNow.AddDays(1),
                NotBefore = DateTimeOffset.UtcNow,
                KeySize = 2048,
                KeyOperations = {KeyOperation.Decrypt, KeyOperation.Encrypt},
                Tags =
                {
                    {"environment", "local"},
                    {"testing", "true"}
                }
            });

            Assert.NotNull(result);

            var createdKey = result.Value;
            Assert.NotNull(createdKey);

            var cryptoClient = new CryptographyClient(createdKey.Id, new LocalTokenCredential());
            var encryptResult = cryptoClient.Encrypt(EncryptionAlgorithm.Rsa15, _encoder.GetBytes(plaintextValue));
            Assert.NotNull(encryptResult);
            Assert.NotNull(encryptResult.Ciphertext);

            var decryptResult = cryptoClient.Decrypt(EncryptionAlgorithm.Rsa15, encryptResult.Ciphertext);
            Assert.NotNull(decryptResult);
            Assert.NotNull(decryptResult.Plaintext);

            var actualDecryptedSecret = _encoder.GetString(decryptResult.Plaintext);
            Assert.Equal(plaintextValue, actualDecryptedSecret);
        }
    }
}
