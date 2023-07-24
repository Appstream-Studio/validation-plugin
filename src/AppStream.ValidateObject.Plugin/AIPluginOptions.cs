using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AppStream.ValidateObject.Plugin;

public class AIPluginOptions
{
    public const string AIPlugin = "AIPlugin";

    [JsonPropertyName("schema_version")]
    [Required]
    public string SchemaVersion { get; set; } = "v1";

    [JsonPropertyName("name_for_model")]
    [Required]
    public string NameForModel { get; set; } = string.Empty;

    [JsonPropertyName("name_for_human")]
    [Required]
    public string NameForHuman { get; set; } = string.Empty;

    [JsonPropertyName("description_for_model")]
    [Required]
    public string DescriptionForModel { get; set; } = string.Empty;

    [JsonPropertyName("description_for_human")]
    [Required]
    public string DescriptionForHuman { get; set; } = string.Empty;

    [JsonPropertyName("auth")]
    [Required]
    public AuthModel Auth { get; set; } = new AuthModel();

    [JsonPropertyName("api")]
    [Required]
    public ApiModel Api { get; set; } = new ApiModel();

    [JsonPropertyName("logo_url")]
    [Required]
    public string LogoUrl { get; set; } = string.Empty;

    [JsonPropertyName("contact_email")]
    [Required]
    public string ContactEmail { get; set; } = string.Empty;

    [JsonPropertyName("legal_info_url")]
    [Required]
    public string LegalInfoUrl { get; set; } = string.Empty;

    public class AuthModel
    {
        [JsonPropertyName("type")]
        [Required]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("authorization_url")]
        [Required]
        public string AuthorizationType { get; set; } = string.Empty;
    }

    public class ApiModel
    {
        [JsonPropertyName("type")]
        [Required]
        public string Type { get; set; } = "openapi";

        [JsonPropertyName("url")]
        [Required]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("has_user_authentication")]
        [Required]
        public bool HasUserAuthentication { get; set; } = false;
    }
}
