using System;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using AzureKeyVaultEmulator.AcceptanceTests.Helpers;
using Xunit;

namespace AzureKeyVaultEmulator.AcceptanceTests.Secrets
{
    public class CreateSecretTests
    {
        private readonly SecretClient _secretClient;

        public CreateSecretTests()
        {
            _secretClient = new SecretClient(new Uri("https://localhost:5551/"), new LocalTokenCredential());
        }

        [Fact]
        public async Task ShouldBeAbleToCreateASecret()
        {
            var secret = new KeyVaultSecret(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
            {
                Properties =
                {
                    Enabled = true,
                    ExpiresOn = DateTimeOffset.UtcNow.AddDays(1),
                    NotBefore = DateTimeOffset.UtcNow,
                    Tags =
                    {
                        { "environment", "local" },
                        { "testing", "true" }
                    }
                }
            };

            var result = await _secretClient.SetSecretAsync(secret);
            Assert.NotNull(result);

            KeyVaultSecret createdSecret = result.Value;
            Assert.NotNull(createdSecret);

            Assert.NotNull(createdSecret.Id);
            Assert.Equal(secret.Value, createdSecret.Value);
            Assert.Equal(secret.Properties.Enabled, createdSecret.Properties.Enabled);
            Assert.Equal(secret.Properties.ExpiresOn.Value.ToUnixTimeSeconds(), createdSecret.Properties.ExpiresOn.GetValueOrDefault().ToUnixTimeSeconds());
            Assert.Equal(secret.Properties.NotBefore.Value.ToUnixTimeSeconds(), createdSecret.Properties.NotBefore.GetValueOrDefault().ToUnixTimeSeconds());
            Assert.NotNull(createdSecret.Properties.Version);
            Assert.Equal("local", createdSecret.Properties.Tags["environment"]);
            Assert.Equal("true", createdSecret.Properties.Tags["testing"]);
        }
    }
}
