using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AzureKeyVaultEmulator.Secrets.Models
{
    public class SetSecretModel
    {
        [JsonPropertyName("value")]
        [Required]
        public string Value { get; set; }

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }

        [JsonPropertyName("attributes")]
        public SecretAttributesModel SecretAttributes { get; set; }

        [JsonPropertyName("tags")]
        public object Tags { get; set; }
    }
}
