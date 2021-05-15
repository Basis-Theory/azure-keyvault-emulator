using System;
using System.Threading.Tasks;
using Azure;
using Azure.Security.KeyVault.Secrets;
using AzureKeyVaultEmulator.AcceptanceTests.Helpers;
using Xunit;

namespace AzureKeyVaultEmulator.AcceptanceTests.Secrets
{
    public class GetSecretTests
    {
        private readonly SecretClient _secretClient;

        public GetSecretTests()
        {
            _secretClient = new SecretClient(new Uri("https://localhost:5551/"), new LocalTokenCredential());
        }

        [Fact]
        public async Task ShouldBeAbleToGetLatestSecretVersionByName()
        {
            var expectedName = Guid.NewGuid().ToString();

            var secret1 = await CreateSecret(expectedName);
            var actualLatest = await _secretClient.GetSecretAsync(expectedName);
            Assert.Equal(secret1.Value.Id, actualLatest.Value.Id);
            Assert.Equal(secret1.Value.Value, actualLatest.Value.Value);
            Assert.Equal(secret1.Value.Properties.Enabled, actualLatest.Value.Properties.Enabled);
            Assert.Equal(secret1.Value.Properties.NotBefore, actualLatest.Value.Properties.NotBefore);
            Assert.Equal(secret1.Value.Properties.ExpiresOn, actualLatest.Value.Properties.ExpiresOn);

            var secret2 = await CreateSecret(expectedName);
            actualLatest = await _secretClient.GetSecretAsync(expectedName);
            Assert.Equal(secret2.Value.Id, actualLatest.Value.Id);
            Assert.Equal(secret2.Value.Value, actualLatest.Value.Value);
            Assert.Equal(secret2.Value.Properties.Enabled, actualLatest.Value.Properties.Enabled);
            Assert.Equal(secret2.Value.Properties.NotBefore, actualLatest.Value.Properties.NotBefore);
            Assert.Equal(secret2.Value.Properties.ExpiresOn, actualLatest.Value.Properties.ExpiresOn);
        }

        [Fact]
        public async Task ShouldBeAbleToGetSecretByNameAndVersion()
        {
            var expectedName = Guid.NewGuid().ToString();
            var expectedSecret = await CreateSecret(expectedName);

            var actualLatestSecret = await _secretClient.GetSecretAsync(expectedName, expectedSecret.Value.Properties.Version);
            Assert.Equal(expectedSecret.Value.Id, actualLatestSecret.Value.Id);
        }

        [Fact]
        public async Task ShouldBeAbleToGetSecretByNameAndVersionWhenNewerVersionExists()
        {
            var expectedName = Guid.NewGuid().ToString();
            var expected = await CreateSecret(expectedName);

            await CreateSecret(expectedName);

            var actualLatest = await _secretClient.GetSecretAsync(expectedName, expected.Value.Properties.Version);
            Assert.Equal(expected.Value.Id, actualLatest.Value.Id);
        }

        [Fact]
        public async Task ShouldThrowRequestFailedExceptionWhenNameDoesNotExist()
        {
            var name = Guid.NewGuid().ToString();
            var exceptionThrown = false;

            try
            {
                await _secretClient.GetSecretAsync(name);
            }
            catch (RequestFailedException)
            {
                exceptionThrown = true;
            }

            Assert.True(exceptionThrown);
        }

        [Fact]
        public async Task ShouldThrowRequestFailedExceptionWhenGetSecretByNameAndVersionDoesNotExist()
        {
            var name = Guid.NewGuid().ToString();
            var version = Guid.NewGuid().ToString();
            var exceptionThrown = false;

            try
            {
                await _secretClient.GetSecretAsync(name, version);
            }
            catch (RequestFailedException)
            {
                exceptionThrown = true;
            }

            Assert.True(exceptionThrown);
        }

        private async Task<Response<KeyVaultSecret>> CreateSecret(string name)
        {
            return await _secretClient.SetSecretAsync(new KeyVaultSecret(name, Guid.NewGuid().ToString())
            {
                Properties =
                {
                    Enabled = true,
                    ExpiresOn = DateTimeOffset.UtcNow.AddDays(1),
                    NotBefore = DateTimeOffset.UtcNow,
                    Tags =
                    {
                        {"environment", "local"},
                        {"testing", "true"}
                    }
                }
            });
        }
    }
}
