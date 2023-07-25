using System.Net;
using AppStream.ValidateObject.Plugin.Requests.ValidateXmlFormat;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace AppStream.ValidateObject.Plugin.Functions.XmlFormatValidation;

public class XmlFormatValidation
{
    public const string FunctionName = "ValidateXmlFormat";

    private readonly IMediator _mediator;

    public XmlFormatValidation(IMediator mediator)
    {
        this._mediator = mediator;
    }

    [Function(FunctionName)]
    [OpenApiOperation(operationId: FunctionName, tags: new[] { "ExecuteFunction" }, Description = "When there is no XML schema provided - checks if provided string is a correct XML.")]
    [OpenApiParameter(name: "xml", Description = "Escaped XML to validate it's format", Type = typeof(string), In = ParameterLocation.Query, Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns information about whether the input string is a valid XML and error list in case it's not.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the error of the input.")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        var xml = req.Query["xml"];

        var request = new ValidateXmlFormat(xml!);
        var result = await this._mediator.Send(request);
        var responseContent = result.IsValid
            ? "This is a valid XML."
            : $"This is not a valid XML - parsing has failed (error: {result.ErrorMessage}).";

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain");
        response.WriteString(responseContent);

        return response;
    }
}
