using System.Text.Json.Serialization;

namespace AzureKeyVaultEmulator.Models
{
    public class KeyAttributesModel
    {
        [JsonPropertyName("created")]
        public int Created { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("exp")]
        public int Expiration { get; set; }

        [JsonPropertyName("nbf")]
        public int NotBefore { get; set; }

        [JsonPropertyName("recoverableDays")]
        public int RecoverableDays { get; set; }

        [JsonPropertyName("recoveryLevel")]
        public string RecoveryLevel { get; set; }

        [JsonPropertyName("updated")]
        public int Updated { get; set; }
    }
}
