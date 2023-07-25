using System.Text.Json.Nodes;
using Json.Schema;
using MediatR;

namespace AppStream.ValidateObject.Plugin.Requests.EvaluateJsonSchema;

internal sealed class EvaluateJsonSchemaHandler : IRequestHandler<EvaluateJsonSchema, JsonSchemaEvaluationResult>
{
    public Task<JsonSchemaEvaluationResult> Handle(EvaluateJsonSchema request, CancellationToken cancellationToken)
    {
        var schema = JsonSchema.FromText(request.JsonSchema);
        var jsonNode = JsonNode.Parse(request.JsonInstance);

        var evaluationResult = schema.Evaluate(jsonNode, new EvaluationOptions { OutputFormat = OutputFormat.List });

        string? errorMessage = null;
        if (evaluationResult.HasDetails)
        {
            var errors = evaluationResult.Details.Where(d => d.HasErrors);
            var formattedErrors = errors.Select(e => $"- {e.InstanceLocation}: {string.Join("; ", e.Errors!.Values)}");

            errorMessage = string.Join("\n", formattedErrors);
        }

        return Task.FromResult(new JsonSchemaEvaluationResult(evaluationResult.IsValid, errorMessage));
    }
}
