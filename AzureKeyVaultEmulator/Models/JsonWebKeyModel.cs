using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;

namespace AzureKeyVaultEmulator.Models
{
    public class JsonWebKeyModel
    {
        [JsonPropertyName("crv")]
        public string KeyCurve { get; set; }

        [JsonPropertyName("d")]
        [JsonIgnore]
        public string D { get; set; }

        [JsonPropertyName("dp")]
        public string Dp { get; set; }

        [JsonPropertyName("dq")]
        public string Dq { get; set; }

        [JsonPropertyName("e")]
        public string E { get; set; }

        [JsonPropertyName("k")]
        public string K { get; set; }

        [JsonPropertyName("key_hsm")]
        public string KeyHsm { get; set; }

        [JsonPropertyName("key_ops")]
        public List<string> KeyOperations { get; set; }

        [JsonPropertyName("kty")]
        public string KeyType { get; set; }

        [JsonPropertyName("kid")]
        public string KeyIdentifier { get; set; }

        [JsonIgnore]
        public string KeyName { get; set; }

        [JsonIgnore]
        public Guid KeyVersion { get; set; }

        [JsonPropertyName("n")]
        public string N { get; set; }

        [JsonPropertyName("p")]
        public string P { get; set; }

        [JsonPropertyName("q")]
        public string Q { get; set; }

        [JsonPropertyName("qi")]
        public string Qi { get; set; }

        [JsonPropertyName("x")]
        public string x { get; set; }

        [JsonPropertyName("y")]
        public string y { get; set; }

        private readonly RSA _rsaKey;
        private readonly RSAParameters _rsaParameters;

        public JsonWebKeyModel()
        {
        }

        public JsonWebKeyModel(RSA rsaKey)
        {
            _rsaKey = rsaKey;
            _rsaParameters = rsaKey.ExportParameters(true);
            D = Base64UrlEncoder.Encode(_rsaParameters.D);
            Dp = Base64UrlEncoder.Encode(_rsaParameters.DP);
            Dq = Base64UrlEncoder.Encode(_rsaParameters.DQ);
            E = Base64UrlEncoder.Encode(_rsaParameters.Exponent);
            D = Base64UrlEncoder.Encode(_rsaParameters.D);
            KeyType = "RSA";
            N = Base64UrlEncoder.Encode(_rsaParameters.Modulus);
            P = Base64UrlEncoder.Encode(_rsaParameters.P);
            Q = Base64UrlEncoder.Encode(_rsaParameters.Q);
            Qi = Base64UrlEncoder.Encode(_rsaParameters.InverseQ);
        }

        public byte[] Encrypt(KeyOperationParameters data)
        {
            switch (KeyType)
            {
                case "RSA":
                {
                    using var rsaAlg = new RSACryptoServiceProvider(_rsaKey.KeySize);
                    rsaAlg.ImportParameters(_rsaParameters);
                    return rsaAlg.Encrypt(Base64UrlEncoder.DecodeBytes(data.Data), false);
                }
                default:
                    throw new NotImplementedException($"KeyType {KeyType} does not support Encryption");
            }
        }

        public string Decrypt(KeyOperationParameters data)
        {
            switch (KeyType)
            {
                case "RSA":
                {
                    using var rsaAlg = new RSACryptoServiceProvider(_rsaKey.KeySize);
                    rsaAlg.ImportParameters(_rsaParameters);
                    return Base64UrlEncoder.Encode(rsaAlg.Decrypt(Base64UrlEncoder.DecodeBytes(data.Data), false));
                }
                default:
                    throw new NotImplementedException($"KeyType {KeyType} does not support Encryption");
            }
        }
    }
}
