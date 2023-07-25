using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace AppStream.ValidateObject.Plugin.Json.FormatValidation;

public class FormatValidation
{
    public const string FunctionName = "ValidateJsonFormat";

    [Function(FunctionName)]
    [OpenApiOperation(operationId: FunctionName, tags: new[] { "ExecuteFunction" }, Description = "Checks if provided JSON string is valid.")]
    [OpenApiParameter(name: "json", Description = "escaped JSON to validate it's format", Type = typeof(string), In = ParameterLocation.Query, Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns information about whether the input string is a valid JSON and error list in case it's not.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the error of the input.")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        var response = req.CreateResponse();
        response.Headers.Add("Content-Type", "text/plain");

        var json = req.Query["json"];
        if (json == null)
        {
            response.StatusCode = HttpStatusCode.BadRequest;
            response.WriteString("JSON parameter is missing in query.");
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
