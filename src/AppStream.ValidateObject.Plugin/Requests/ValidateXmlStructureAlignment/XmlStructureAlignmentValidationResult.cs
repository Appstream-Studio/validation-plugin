namespace AppStream.ValidateObject.Plugin.Requests.ValidateXmlStructureAlignment;

internal sealed record XmlStructureAlignmentValidationResult(bool IsValid, string[] Errors);
