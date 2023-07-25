using System.Net;
using System.Text;
using AppStream.ValidateObject.Plugin.Requests.ValidateJsonStructureAlignment;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace AppStream.ValidateObject.Plugin.Functions.StructureAlignmentValidation;

public class StructureAlignmentValidation
{
    public const string FunctionName = "ValidateJsonsStructureAlignment";

    private readonly IMediator _mediator;

    public StructureAlignmentValidation(IMediator mediator)
    {
        this._mediator = mediator;
    }

    [Function(FunctionName)]
    [OpenApiOperation(operationId: FunctionName, tags: new[] { "ExecuteFunction" }, Description = "When provided with two JSON objects - checks if their structure is identical.")]
    [OpenApiParameter(name: "json1", Description = "First escaped JSON object", Type = typeof(string), In = ParameterLocation.Query, Required = true)]
    [OpenApiParameter(name: "json2", Description = "Second escaped JSON object", Type = typeof(string), In = ParameterLocation.Query, Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns plain text information about whether given JSON objects' structures are identical.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the error of the input.")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req, ILogger logger)
    {
        var json1 = req.Query["json1"];
        var json2 = req.Query["json2"];

        var request = new ValidateJsonStructureAlignment(json1!, json2!);
        var validationResult = await this._mediator.Send(request);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain");
        response.WriteString(FormatResponse(validationResult));

        return response;
    }

    private static string FormatResponse(JsonStructureAlignmentValidationResult validationResult)
    {
        var responseContent = validationResult.IsValid
            ? "Provided JSONs are of identical structure."
            : "Provided JSONs are not of identical structure.";

        if (!validationResult.IsValid && validationResult.Errors.Any())
        {
            var sb = new StringBuilder(responseContent);
            sb.AppendLine(" Here are the differences of the second JSON's structure compared to the first in (path, error) format:");
            foreach (var error in validationResult.Errors)
            {
                sb.AppendLine($"- ({error.Path}, {error.Error})");
            }

            responseContent = sb.ToString();
        }

        return responseContent;
    }
}
