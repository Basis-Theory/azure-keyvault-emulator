using System.Text.Json.Serialization;

namespace LocalAzureKeyVaultSpike.Models
{
    public class KeyOperationParameters
    {
        [JsonPropertyName("alg")]
        public string Algorithm { get; set; }

        [JsonPropertyName("value")]
        public string Data { get; set; }
    }
}
