namespace AppStream.ValidateObject.Plugin.Requests.EvaluateXmlSchema;

internal sealed record XmlSchemaEvaluationResult(bool IsValid, string[] Errors);
