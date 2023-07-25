using MediatR;
using NJsonSchema;
using NJsonSchema.Validation;

namespace AppStream.ValidateObject.Plugin.Requests.ValidateJsonStructureAlignment;

internal sealed class ValidateJsonStructureAlignmentHandler : IRequestHandler<ValidateJsonStructureAlignment, JsonStructureAlignmentValidationResult>
{
    public Task<JsonStructureAlignmentValidationResult> Handle(ValidateJsonStructureAlignment request, CancellationToken cancellationToken)
    {
        var schema = JsonSchema.FromSampleJson(request.Json1);
        schema.AllowAdditionalItems = false;
        schema.AllowAdditionalProperties = false;
        foreach (var property in schema.Properties)
        {
            schema.RequiredProperties.Add(property.Key);
        }

        var validator = new JsonSchemaValidator();
        var errors = validator.Validate(request.Json2, schema);

        var resut = new JsonStructureAlignmentValidationResult(
            IsValid: !errors.Any(),
            Errors: errors
                .Select(e => new JsonStructureAlignmentValidationError(
                    Path: e.Path,
                    Error: e.Kind.ToString()))
                .ToArray());

        return Task.FromResult(resut);
    }
}
