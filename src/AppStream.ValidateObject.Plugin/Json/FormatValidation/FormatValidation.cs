using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace AppStream.ValidateObject.Plugin.Json.FormatValidation;

public class FormatValidation
{
    public const string FunctionName = "ValidateJsonFormat";

    [Function(FunctionName)]
    [OpenApiOperation(operationId: FunctionName, tags: new[] { "ExecuteFunction" }, Description = "Checks if a string is a valid JSON.")]
    [OpenApiRequestBody("text/plain", typeof(string), Description = "String to be examined.", Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns plain text information about whether the input string is a valid JSON or not.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the error of the input.")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        var response = req.CreateResponse();
        response.Headers.Add("Content-Type", "text/plain");

        var json = req.ReadAsString();
        if (json == null)
        {
            response.StatusCode = HttpStatusCode.BadRequest;
            response.WriteString("Request body is empty.");
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
