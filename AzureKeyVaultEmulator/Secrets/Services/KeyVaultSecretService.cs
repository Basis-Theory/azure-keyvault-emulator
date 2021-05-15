using System;
using System.Collections.Concurrent;
using AzureKeyVaultEmulator.Secrets.Models;

namespace AzureKeyVaultEmulator.Secrets.Services
{
    public interface IKeyVaultSecretService
    {
        SecretResponse Get(string name);
        SecretResponse Get(string name, string version);
        SecretResponse SetSecret(string name, SetSecretModel requestBody);
    }

    public class KeyVaultSecretService : IKeyVaultSecretService
    {
        private static readonly ConcurrentDictionary<string, SecretResponse> Secrets = new();

        public SecretResponse Get(string name)
        {
            Secrets.TryGetValue(GetSecretCacheId(name), out var found);

            return found;
        }

        public SecretResponse Get(string name, string version)
        {
            Secrets.TryGetValue(GetSecretCacheId(name, version), out var found);

            return found;
        }

        public SecretResponse SetSecret(string name, SetSecretModel secret)
        {
            var version = Guid.NewGuid().ToString();

            var response = new SecretResponse
            {
                Value = secret.Value,
                Attributes = secret.SecretAttributes,
                Tags = secret.Tags
            };

            Secrets.AddOrUpdate(GetSecretCacheId(name), response, (_, _) => response);
            Secrets.TryAdd(GetSecretCacheId(name, version), response);

            return response;
        }

        private static string GetSecretCacheId(string name, string version = null) => name + (version ?? "");
    }
}
