namespace AppStream.ValidateObject.Plugin.Requests.ValidateXmlFormat;

internal sealed record XmlFormatValidationResult(bool IsValid, string? ErrorMessage);
