using System;
using System.Text;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using AzureKeyVaultEmulator.AcceptanceTests.Helpers;
using Xunit;

namespace AzureKeyVaultEmulator.AcceptanceTests
{
    public class CreateKeyTests
    {
        private readonly KeyClient _keyClient;
        private readonly UTF8Encoding _encoder = new();

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

            _validateEncryptDecrypt(plaintextValue, cryptoClient, EncryptionAlgorithm.Rsa15);
            _validateEncryptDecrypt(plaintextValue, cryptoClient, EncryptionAlgorithm.RsaOaep);
        }

        private void _validateEncryptDecrypt(string plaintext, CryptographyClient cryptoClient,
            EncryptionAlgorithm algorithm)
        {
            var encryptResult = cryptoClient.Encrypt(algorithm, _encoder.GetBytes(plaintext));
            Assert.NotNull(encryptResult);
            Assert.NotNull(encryptResult.Ciphertext);

            var decryptResult = cryptoClient.Decrypt(algorithm, encryptResult.Ciphertext);
            Assert.NotNull(decryptResult);
            Assert.NotNull(decryptResult.Plaintext);

            var actualDecryptedSecret = _encoder.GetString(decryptResult.Plaintext);
            Assert.Equal(plaintext, actualDecryptedSecret);
        }
    }
}
