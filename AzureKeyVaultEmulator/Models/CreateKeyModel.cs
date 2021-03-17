using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AzureKeyVaultEmulator.Models
{
    public class CreateKeyModel
    {
        [JsonPropertyName("kty")]
        [Required]
        public string KeyType { get; set; }

        [JsonPropertyName("attributes")]
        public KeyAttributesModel KeyAttributes { get; set; }

        [JsonPropertyName("crv")]
        public string KeyCurveName { get; set; }

        [JsonPropertyName("key_ops")]
        public List<string> KeyOperations { get; set; }

        [JsonPropertyName("key_size")]
        public int? KeySize { get; set; }

        [JsonPropertyName("tags")]
        public object Tags { get; set; }
    }
}
