using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace AppStream.Validation.Plugin.Json;

public class JsonValidation
{
    public const string FunctionName = "is_json_valid";
    private const string _jsonQueryParameterName = "json";

    private readonly ILogger<JsonValidation> _logger;

    public JsonValidation(ILogger<JsonValidation> logger)
    {
        this._logger = logger;
    }

    [Function(FunctionName)]
    [OpenApiOperation(operationId: FunctionName, tags: new[] { "ExecuteFunction" }, Description = "Checks if a string is a valid JSON.")]
    [OpenApiParameter(name: _jsonQueryParameterName, Description = "String to be examined.", Required = true, In = ParameterLocation.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns True if the string is a valid JSON, false otherwise.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the error of the input.")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        var response = req.CreateResponse();
        response.Headers.Add("Content-Type", "text/plain");

        string? json = req.Query[_jsonQueryParameterName];
        if (json == null)
        {
            response.StatusCode = HttpStatusCode.BadRequest;
            response.WriteString($"Required query parameter '{_jsonQueryParameterName}' is missing.");
        }
        else
        {
            response.StatusCode = HttpStatusCode.OK;

            var result = this.IsValid(json);
            var responseContent = result.IsValid
                ? "This is a valid JSON."
                : $"This is not a valid JSON - parsing has failed (error: {result.ExceptionMessage}).";

            response.WriteString(responseContent);
        }

        return response;
    }

    private (bool IsValid, string? ExceptionMessage) IsValid(string value)
    {
        try
        {
            JsonDocument.Parse(value);
            return (true, null);
        }
        catch (JsonException e)
        {
            return (false, e.Message);
        }
    }
}
