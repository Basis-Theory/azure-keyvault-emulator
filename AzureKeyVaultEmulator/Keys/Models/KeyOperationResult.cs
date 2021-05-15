using System.Text.Json.Serialization;

namespace AzureKeyVaultEmulator.Keys.Models
{
    public class KeyOperationResult
    {
        [JsonPropertyName("kid")]
        public string KeyIdentifier { get; set; }

        [JsonPropertyName("value")]
        public string Data { get; set; }
    }
}
