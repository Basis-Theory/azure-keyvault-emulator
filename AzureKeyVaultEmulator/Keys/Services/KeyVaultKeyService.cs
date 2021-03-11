using System;
using System.Collections.Concurrent;
using AzureKeyVaultEmulator.Keys.Factories;
using AzureKeyVaultEmulator.Keys.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace AzureKeyVaultEmulator.Keys.Services
{
    public interface IKeyVaultKeyService
    {
        KeyResponse Get(string name);
        KeyResponse Get(string name, string version);
        KeyResponse CreateKey(string name, CreateKeyModel key);

        KeyOperationResult Encrypt(string name, string version, KeyOperationParameters keyOperationParameters);
        KeyOperationResult Decrypt(string keyName, string keyVersion, KeyOperationParameters keyOperationParameters);
    }

    public class KeyVaultKeyService : IKeyVaultKeyService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly ConcurrentDictionary<string, KeyResponse> Keys = new();

        public KeyVaultKeyService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public KeyResponse Get(string name)
        {
            Keys.TryGetValue(GetCacheId(name), out var found);

            return found;
        }

        public KeyResponse Get(string name, string version)
        {
            Keys.TryGetValue(GetCacheId(name, version), out var found);

            return found;
        }

        public KeyResponse CreateKey(string name, CreateKeyModel key)
        {
            JsonWebKeyModel jsonWebKeyModel;
            switch (key.KeyType)
            {
                case "RSA":
                    var rsaKey = RsaKeyFactory.CreateRsaKey(key.KeySize);
                    jsonWebKeyModel = new JsonWebKeyModel(rsaKey);
                    break;

                default:
                    throw new NotImplementedException($"KeyType {key.KeyType} is not supported");
            }

            var version = Guid.NewGuid().ToString();
            var keyUrl = new UriBuilder
            {
                Scheme = _httpContextAccessor.HttpContext.Request.Scheme,
                Host = _httpContextAccessor.HttpContext.Request.Host.Host,
                Port = _httpContextAccessor.HttpContext.Request.Host.Port ?? -1,
                Path = $"keys/{name}/{version}"
            };

            jsonWebKeyModel.KeyName = name;
            jsonWebKeyModel.KeyVersion = version;
            jsonWebKeyModel.KeyIdentifier = keyUrl.Uri.ToString();
            jsonWebKeyModel.KeyOperations = key.KeyOperations;

            var response = new KeyResponse
            {
                Key = jsonWebKeyModel,
                Attributes = key.KeyAttributes,
                Tags = key.Tags
            };

            Keys.AddOrUpdate(GetCacheId(name), response, (_, _) => response);
            Keys.TryAdd(GetCacheId(name, version), response);

            return response;
        }

        public KeyOperationResult Encrypt(string name, string version, KeyOperationParameters keyOperationParameters)
        {
            if (!Keys.TryGetValue(GetCacheId(name, version), out var foundKey))
                throw new Exception("Key not found");

            var encrypted = Base64UrlEncoder.Encode(foundKey.Key.Encrypt(keyOperationParameters));

            return new KeyOperationResult
            {
                KeyIdentifier = foundKey.Key.KeyIdentifier,
                Data = encrypted
            };
        }

        public KeyOperationResult Decrypt(string keyName, string keyVersion, KeyOperationParameters keyOperationParameters)
        {
            if (!Keys.TryGetValue(GetCacheId(keyName, keyVersion), out var foundKey))
                throw new Exception("Key not found");

            var decrypted = foundKey.Key.Decrypt(keyOperationParameters);

            return new KeyOperationResult
            {
                KeyIdentifier = foundKey.Key.KeyIdentifier,
                Data = decrypted
            };
        }

        private static string GetCacheId(string name, string version = null) => name + (version ?? "");
    }
}
