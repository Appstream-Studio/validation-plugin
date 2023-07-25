namespace AppStream.ValidateObject.Plugin.Requests.ValidateJsonStructureAlignment;

internal sealed record JsonStructureAlignmentValidationResult(bool IsValid, JsonStructureAlignmentValidationError[] Errors);

internal sealed record JsonStructureAlignmentValidationError(string Path, string Error);
