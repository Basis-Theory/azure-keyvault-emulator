using System;
using System.Text;
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

            Assert.Equal(secret.Value, result.Value.Value);
            Assert.Equal(secret.Properties.Enabled, result.Value.Properties.Enabled);
            Assert.Equal(secret.Properties.ExpiresOn.Value.ToUnixTimeSeconds(), result.Value.Properties.ExpiresOn.GetValueOrDefault().ToUnixTimeSeconds());
            Assert.Equal(secret.Properties.NotBefore.Value.ToUnixTimeSeconds(), result.Value.Properties.NotBefore.GetValueOrDefault().ToUnixTimeSeconds());
            Assert.Equal("local", result.Value.Properties.Tags["environment"]);
            Assert.Equal("true", result.Value.Properties.Tags["testing"]);
        }
    }
}
