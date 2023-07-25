using MediatR;

namespace AppStream.ValidateObject.Plugin.Requests.ValidateJsonFormat;

internal sealed record ValidateJsonFormat(string Json) : IRequest<JsonFormatValidationResult>;
