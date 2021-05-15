using System.Text.Json.Serialization;

namespace AzureKeyVaultEmulator.Secrets.Models
{
    public class SecretAttributesModel
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("exp")]
        public int Expiration { get; set; }

        [JsonPropertyName("nbf")]
        public int NotBefore { get; set; }
    }
}
