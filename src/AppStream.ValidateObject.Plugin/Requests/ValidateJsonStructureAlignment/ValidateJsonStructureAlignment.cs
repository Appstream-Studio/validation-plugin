using MediatR;

namespace AppStream.ValidateObject.Plugin.Requests.ValidateJsonStructureAlignment;

internal sealed record ValidateJsonStructureAlignment(string Json1, string Json2) : IRequest<JsonStructureAlignmentValidationResult>;
