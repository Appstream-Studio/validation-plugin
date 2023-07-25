namespace AppStream.ValidateObject.Plugin.Requests.EvaluateJsonSchema;

internal sealed record JsonSchemaEvaluationResult(bool IsValid, string? ErrorMessage);
