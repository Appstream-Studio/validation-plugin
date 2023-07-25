using System.Net;
using System.Text;
using AppStream.ValidateObject.Plugin.Requests.EvaluateJsonSchema;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace AppStream.ValidateObject.Plugin.Functions.SchemaValidation;

public class SchemaValidation
{
    public const string FunctionName = "ValidateJsonAgainstSchema";

    private readonly IMediator _mediator;

    public SchemaValidation(IMediator mediator)
    {
        this._mediator = mediator;
    }

    [Function(FunctionName)]
    [OpenApiOperation(operationId: FunctionName, tags: new[] { "ExecuteFunction" }, Description = "When provided both JSON object to verify and JSON schema to verify against - checks if the object satisfies the given schema.")]
    [OpenApiParameter(name: "jsonInstance", Description = "Escaped JSON object to be validated against schema", Type = typeof(string), In = ParameterLocation.Query, Required = true)]
    [OpenApiParameter(name: "jsonSchema", Description = "Escaped JSON schema to be used to validate the object", Type = typeof(string), In = ParameterLocation.Query, Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns plain text information about whether the given JSON satisfies given JSON schema.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the error of the input.")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req, ILogger logger)
    {
        var jsonInstance = req.Query["jsonInstance"];
        var jsonSchema = req.Query["jsonSchema"];

        var request = new EvaluateJsonSchema(jsonInstance!, jsonSchema!);
        var validationResult = await this._mediator.Send(request);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain");
        response.WriteString(FormatResponse(validationResult));

        return response;
    }

    private static string FormatResponse(JsonSchemaEvaluationResult validationResult)
    {
        var responseContent = validationResult.IsValid
            ? "This JSON satisfies given JSON schema."
            : "This JSON does not satisfy given JSON schema.";

        if (!validationResult.IsValid && validationResult.Errors.Any())
        {
            var sb = new StringBuilder(responseContent);
            sb.AppendLine(" Here are the errors in (path, error) format:");
            foreach (var error in validationResult.Errors)
            {
                sb.AppendLine($"- ({error.Path}, {error.Error})");
            }

            responseContent = sb.ToString();
        }

        return responseContent;
    }
}
