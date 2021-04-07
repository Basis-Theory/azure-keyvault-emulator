using System;
using System.Collections.Concurrent;
using AzureKeyVaultEmulator.Factories;
using AzureKeyVaultEmulator.Models;
using Microsoft.IdentityModel.Tokens;

namespace AzureKeyVaultEmulator.Services
{
    public interface IKeyVaultKeyService
    {
        KeyResponse GetKey(string keyName);
        KeyResponse GetKey(string keyName, Guid keyVersion);
        KeyResponse CreateKeyVaultKey(string keyName, CreateKeyModel key);

        KeyOperationResult Encrypt(string keyName, Guid keyVersion, KeyOperationParameters keyOperationParameters);
        KeyOperationResult Decrypt(string keyName, Guid keyVersion, KeyOperationParameters keyOperationParameters);
    }

    public class KeyVaultKeyService : IKeyVaultKeyService
    {
        private static readonly ConcurrentDictionary<string, KeyResponse> Keys = new();
        private readonly string _keyVaultName;

        public KeyVaultKeyService(string keyVaultName)
        {
            _keyVaultName = keyVaultName ?? throw new ArgumentNullException(nameof(keyVaultName));
        }

        public KeyResponse GetKey(string keyName)
        {
            Keys.TryGetValue(GetKeyCacheId(keyName), out var foundKey);

            return foundKey;
        }

        public KeyResponse GetKey(string keyName, Guid keyVersion)
        {
            Keys.TryGetValue(GetKeyCacheId(keyName, keyVersion), out var foundKey);

            return foundKey;
        }

        public KeyResponse CreateKeyVaultKey(string keyName, CreateKeyModel key)
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

            var keyVersion = Guid.NewGuid();
            jsonWebKeyModel.KeyName = keyName;
            jsonWebKeyModel.KeyVersion = keyVersion;
            jsonWebKeyModel.KeyIdentifier = $"https://{_keyVaultName}:5001/keys/{keyName}/{keyVersion}";
            jsonWebKeyModel.KeyOperations = key.KeyOperations;

            var createdKey = new KeyResponse
            {
                Attributes = key.KeyAttributes,
                Key = jsonWebKeyModel,
                Tags = key.Tags
            };

            Keys.AddOrUpdate(GetKeyCacheId(keyName), createdKey, (_, _) => createdKey);
            Keys.TryAdd(GetKeyCacheId(keyName, keyVersion), createdKey);

            return createdKey;
        }

        public KeyOperationResult Encrypt(string keyName, Guid keyVersion, KeyOperationParameters keyOperationParameters)
        {
            if (!Keys.TryGetValue(GetKeyCacheId(keyName, keyVersion), out var foundKey))
                throw new Exception("Key not found");

            var encrypted = Base64UrlEncoder.Encode(foundKey.Key.Encrypt(keyOperationParameters));

            return new KeyOperationResult
            {
                KeyIdentifier = foundKey.Key.KeyIdentifier,
                Data = encrypted
            };
        }

        public KeyOperationResult Decrypt(string keyName, Guid keyVersion, KeyOperationParameters keyOperationParameters)
        {
            if (!Keys.TryGetValue(GetKeyCacheId(keyName, keyVersion), out var foundKey))
                throw new Exception("Key not found");

            var decrypted = foundKey.Key.Decrypt(keyOperationParameters);

            return new KeyOperationResult
            {
                KeyIdentifier = foundKey.Key.KeyIdentifier,
                Data = decrypted
            };
        }

        private static string GetKeyCacheId(string keyName, Guid? keyVersion = null) => keyName + (keyVersion == null ? "" : keyVersion.ToString());
    }
}
