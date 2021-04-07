using System;
using Azure;
using Azure.Security.KeyVault.Keys;
using AzureKeyVaultEmulator.AcceptanceTests.Helpers;
using Xunit;

namespace AzureKeyVaultEmulator.AcceptanceTests
{
    public class GetKeyTests
    {
        private readonly KeyClient _keyClient;

        public GetKeyTests()
        {
            _keyClient = new KeyClient(new Uri("https://localhost:5001/"), new LocalTokenCredential());
        }

        [Fact]
        public void ShouldBeAbleToGetLatestKeyVersionByKeyName()
        {
            const string expectedKeyName = "foo-rsa";
            Response<KeyVaultKey> actualLatestKey;

            var key1 = CreateKeyVaultRsaKey(expectedKeyName);
            actualLatestKey = _keyClient.GetKey(expectedKeyName);
            Assert.Equal(key1.Value.Id, actualLatestKey.Value.Id);

            var key2 = CreateKeyVaultRsaKey(expectedKeyName);
            actualLatestKey = _keyClient.GetKey(expectedKeyName);
            Assert.Equal(key2.Value.Id, actualLatestKey.Value.Id);
        }

        [Fact]
        public void ShouldBeAbleToGetKeyByKeyNameAndKeyVersion()
        {
            const string expectedKeyName = "foo-rsa";
            var expectedKey = CreateKeyVaultRsaKey(expectedKeyName);

            var actualLatestKey = _keyClient.GetKey(expectedKeyName, expectedKey.Value.Properties.Version);

            Assert.Equal(expectedKey.Value.Id, actualLatestKey.Value.Id);
        }

        private Response<KeyVaultKey> CreateKeyVaultRsaKey(string keyName)
        {
            return _keyClient.CreateRsaKey(new CreateRsaKeyOptions(keyName)
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
        }
    }
}
