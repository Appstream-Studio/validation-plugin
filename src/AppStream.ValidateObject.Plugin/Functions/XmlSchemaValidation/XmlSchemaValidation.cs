using System.Net;
using System.Text;
using AppStream.ValidateObject.Plugin.Requests.EvaluateXmlSchema;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace AppStream.ValidateObject.Plugin.Functions.XmlSchemaValidation;

public class XmlSchemaValidation
{
    public const string FunctionName = "ValidateXmlAgainstSchema";

    private readonly IMediator _mediator;

    public XmlSchemaValidation(IMediator mediator)
    {
        this._mediator = mediator;
    }

    [Function(FunctionName)]
    [OpenApiOperation(operationId: FunctionName, tags: new[] { "ExecuteFunction" }, Description = "When provided both XML object to verify and XML schema to verify against - checks if the object satisfies the given schema.")]
    [OpenApiParameter(name: "xmlInstance", Description = "Escaped XML object to be validated against schema", Type = typeof(string), In = ParameterLocation.Query, Required = true)]
    [OpenApiParameter(name: "xmlSchema", Description = "Escaped XML schema to be used to validate the object", Type = typeof(string), In = ParameterLocation.Query, Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns plain text information about whether the given XML satisfies given XML schema.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the error of the input.")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req, ILogger logger)
    {
        var xmlInstance = req.Query["xmlInstance"];
        var xmlSchema = req.Query["xmlSchema"];

        var request = new EvaluateXmlSchema(xmlInstance!, xmlSchema!);
        var validationResult = await this._mediator.Send(request);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain");
        response.WriteString(FormatResponse(validationResult));

        return response;
    }

    private static string FormatResponse(XmlSchemaEvaluationResult validationResult)
    {
        var responseContent = validationResult.IsValid
            ? "This XML satisfies given XML schema."
            : "This XML does not satisfy given XML schema.";

        if (!validationResult.IsValid && validationResult.Errors.Any())
        {
            var sb = new StringBuilder(responseContent);
            sb.AppendLine(" Here are the errors:");
            foreach (var error in validationResult.Errors)
            {
                sb.AppendLine($"- {error}");
            }

            responseContent = sb.ToString();
        }

        return responseContent;
    }
}
