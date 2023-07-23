using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace AppStream.Validation.Plugin;

public class IsValidJson
{
    public const string FunctionName = "is-json-valid";
    private const string _jsonQueryParameterName = "json";

    private readonly ILogger _logger;

    public IsValidJson(ILoggerFactory loggerFactory)
    {
        this._logger = loggerFactory.CreateLogger<IsValidJson>();
    }

    [Function(FunctionName)]
    [OpenApiOperation(operationId: FunctionName, tags: new[] { "ExecuteFunction" }, Description = "Checks if a string is a valid JSON.")]
    [OpenApiParameter(name: _jsonQueryParameterName, Description = "String to be examined.", Required = true, In = ParameterLocation.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns True if the string is a valid JSON, false otherwise.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the error of the input.")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        string? json = req.Query[_jsonQueryParameterName];
        if (json == null)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);
            response.Headers.Add("Content-Type", "text/plain");
            response.WriteString($"Required query parameter '{_jsonQueryParameterName}' is missing.");

            return response;
        }
        else
        {
            bool isValid = this.IsValid(json);

            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain");
            response.WriteString(isValid.ToString());

            return response;
        }
    }

    private bool IsValid(string value)
    {
        try
        {
            JsonDocument.Parse(value);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
