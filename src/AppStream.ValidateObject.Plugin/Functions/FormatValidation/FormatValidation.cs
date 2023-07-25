using System.Net;
using AppStream.ValidateObject.Plugin.Requests.ValidateJsonFormat;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace AppStream.ValidateObject.Plugin.Functions.FormatValidation;

public class FormatValidation
{
    public const string FunctionName = "ValidateJsonFormat";

    private readonly IMediator _mediator;

    public FormatValidation(IMediator mediator)
    {
        this._mediator = mediator;
    }

    [Function(FunctionName)]
    [OpenApiOperation(operationId: FunctionName, tags: new[] { "ExecuteFunction" }, Description = "Checks if provided JSON string is valid.")]
    [OpenApiParameter(name: "json", Description = "Escaped JSON to validate it's format", Type = typeof(string), In = ParameterLocation.Query, Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns information about whether the input string is a valid JSON and error list in case it's not.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the error of the input.")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        var json = req.Query["json"];

        var request = new ValidateJsonFormat(json!);
        var result = await this._mediator.Send(request);
        var responseContent = result.IsValid
            ? "This is a valid JSON."
            : $"This is not a valid JSON - parsing has failed (error: {result.ErrorMessage}).";

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain");
        response.WriteString(responseContent);

        return response;
    }
}
