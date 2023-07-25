namespace AppStream.ValidateObject.Plugin.Requests.ValidateJsonFormat;

internal sealed record JsonFormatValidationResult(bool IsValid, string? ErrorMessage);
