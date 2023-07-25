using System.Text.Json;
using MediatR;

namespace AppStream.ValidateObject.Plugin.Requests.ValidateJsonFormat;

internal sealed class ValidateJsonFormatHandler : IRequestHandler<ValidateJsonFormat, JsonFormatValidationResult>
{
    public Task<JsonFormatValidationResult> Handle(ValidateJsonFormat request, CancellationToken cancellationToken)
    {
        JsonFormatValidationResult result;
        try
        {
            JsonDocument.Parse(request.Json);
            result = new JsonFormatValidationResult(true, null);
        }
        catch (JsonException e)
        {
            result = new JsonFormatValidationResult(false, e.Message);
        }

        return Task.FromResult(result);
    }
}
