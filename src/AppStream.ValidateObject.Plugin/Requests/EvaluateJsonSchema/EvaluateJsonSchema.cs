using MediatR;

namespace AppStream.ValidateObject.Plugin.Requests.EvaluateJsonSchema;

internal sealed record EvaluateJsonSchema(string JsonInstance, string JsonSchema) : IRequest<JsonSchemaEvaluationResult>;
