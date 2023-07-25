using System.Net;
using System.Text;
using AppStream.ValidateObject.Plugin.Requests.ValidateXmlStructureAlignment;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace AppStream.ValidateObject.Plugin.Functions.XmlStructureAlignmentValidation;

public class XmlStructureAlignmentValidation
{
    public const string FunctionName = "ValidateXmlsStructureAlignment";

    private readonly IMediator _mediator;

    public XmlStructureAlignmentValidation(IMediator mediator)
    {
        this._mediator = mediator;
    }

    [Function(FunctionName)]
    [OpenApiOperation(operationId: FunctionName, tags: new[] { "ExecuteFunction" }, Description = "When provided with two XML objects - checks if their structure is identical.")]
    [OpenApiParameter(name: "xml1", Description = "First escaped XML object", Type = typeof(string), In = ParameterLocation.Query, Required = true)]
    [OpenApiParameter(name: "xml2", Description = "Second escaped XML object", Type = typeof(string), In = ParameterLocation.Query, Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns plain text information about whether given XML objects' structures are identical.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the error of the input.")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req, ILogger logger)
    {
        var xml1 = req.Query["xml1"];
        var xml2 = req.Query["xml2"];

        var request = new ValidateXmlStructureAlignment(xml1!, xml2!);
        var validationResult = await this._mediator.Send(request);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain");
        response.WriteString(FormatResponse(validationResult));

        return response;
    }

    private static string FormatResponse(XmlStructureAlignmentValidationResult validationResult)
    {
        var responseContent = validationResult.IsValid
            ? "Provided XMLs are of identical structure."
            : "Provided XMLs are not of identical structure.";

        if (!validationResult.IsValid && validationResult.Errors.Any())
        {
            var sb = new StringBuilder(responseContent);
            sb.AppendLine(" Here are the differences of the second XML's structure compared to the first:");
            foreach (var error in validationResult.Errors)
            {
                sb.AppendLine($"- {error}");
            }

            responseContent = sb.ToString();
        }

        return responseContent;
    }
}
