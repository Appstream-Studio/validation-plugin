using System.Text.Json;
using System.Text.Json.Nodes;
using FluentValidation;
using NJsonSchema;

namespace AppStream.ValidateObject.Plugin.Requests.EvaluateJsonSchema;

internal sealed class EvaluateJsonSchemaValidator : AbstractValidator<EvaluateJsonSchema>
{
    public EvaluateJsonSchemaValidator()
    {
        this.RuleFor(r => r.JsonInstance).NotNull().NotEmpty()
            .Must(this.IsValidJson)
            .WithMessage("'{PropertyName}' must be a valid JSON ({ErrorMessage}).");

        this.RuleFor(r => r.JsonSchema).NotNull().NotEmpty()
            .MustAsync(this.IsValidJsonSchema)
            .WithMessage("'{PropertyName}' must be a valid JSON schema ({ErrorMessage}).");
    }

    private bool IsValidJson(EvaluateJsonSchema root, string jsonInstance, ValidationContext<EvaluateJsonSchema> context)
    {
        try
        {
            JsonNode.Parse(jsonInstance);
            return true;
        }
        catch (JsonException e)
        {
            context.MessageFormatter.AppendArgument("ErrorMessage", e.Message);
            return false;
        }
    }

    private async Task<bool> IsValidJsonSchema(EvaluateJsonSchema root, string jsonSchema, ValidationContext<EvaluateJsonSchema> context, CancellationToken ct)
    {
        try
        {
            await JsonSchema.FromJsonAsync(jsonSchema, ct);
            return true;
        }
        catch (Exception e) when (e is JsonException || e is InvalidOperationException)
        {
            context.MessageFormatter.AppendArgument("ErrorMessage", e.Message);
            return false;
        }
    }
}
