using System.Text.Json.Serialization;

namespace LocalAzureKeyVaultSpike.Models
{
    public class KeyOperationResult
    {
        [JsonPropertyName("kid")]
        public string KeyIdentifier { get; set; }

        [JsonPropertyName("value")]
        public string Data { get; set; }
    }
}
