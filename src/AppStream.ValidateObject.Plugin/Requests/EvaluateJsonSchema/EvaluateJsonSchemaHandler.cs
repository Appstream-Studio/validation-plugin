using MediatR;
using NJsonSchema;
using NJsonSchema.Validation;

namespace AppStream.ValidateObject.Plugin.Requests.EvaluateJsonSchema;

internal sealed class EvaluateJsonSchemaHandler : IRequestHandler<EvaluateJsonSchema, JsonSchemaEvaluationResult>
{
    public async Task<JsonSchemaEvaluationResult> Handle(EvaluateJsonSchema request, CancellationToken cancellationToken)
    {
        var schema = await JsonSchema.FromJsonAsync(request.JsonSchema);
        var validator = new JsonSchemaValidator();
        var errors = validator.Validate(request.JsonInstance, schema);

        return new JsonSchemaEvaluationResult(
            IsValid: !errors.Any(),
            Errors: errors
                .Select(e => new JsonSchemaEvaluationError(
                    Path: e.Path,
                    Error: e.Kind.ToString()))
                .ToArray());
    }
}
