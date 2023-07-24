using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace AppStream.ValidateObject.Plugin.Json.SchemaValidation;

public class SchemaValidationRequestBody
{
    [OpenApiProperty(Description = "JSON instance to be evaluated.", Nullable = false)]
    public string? JsonInstance { get; set; }

    [OpenApiProperty(Description = "JSON schema to evaluate the instance against.", Nullable = false)]
    public string? JsonSchema { get; set; }
}
