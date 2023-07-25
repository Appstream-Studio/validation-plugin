using System.Text.Json;
using MediatR;

namespace AppStream.ValidateObject.Plugin.Requests.ValidateJsonFormat;

internal sealed class ValidateJsonFormatHandler : IRequestHandler<ValidateJsonFormat, JsonFormatValidationResult>
{
    public Task<JsonFormatValidationResult> Handle(ValidateJsonFormat request, CancellationToken cancellationToken)
    {
        try
        {
            JsonDocument.Parse(request.Json);
            return Task.FromResult(new JsonFormatValidationResult(true, null));
        }
        catch (JsonException e)
        {
            return Task.FromResult(new JsonFormatValidationResult(false, e.Message));
        }
    }
}
