namespace AppStream.ValidateObject.Plugin.Requests.EvaluateJsonSchema;

internal sealed record JsonSchemaEvaluationResult(bool IsValid, JsonSchemaEvaluationError[] Errors);

internal sealed record JsonSchemaEvaluationError(string Path, string Error);
