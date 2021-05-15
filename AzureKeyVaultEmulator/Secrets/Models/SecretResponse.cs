using System.Text.Json.Serialization;

namespace AzureKeyVaultEmulator.Secrets.Models
{
    public class SecretResponse
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("attributes")]
        public SecretAttributesModel Attributes { get; set; }

        [JsonPropertyName("tags")]
        public object Tags { get; set; }
    }
}
