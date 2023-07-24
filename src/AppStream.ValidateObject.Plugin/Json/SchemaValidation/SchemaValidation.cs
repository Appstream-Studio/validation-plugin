using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace AppStream.ValidateObject.Plugin.Json.SchemaValidation;

public class SchemaValidation
{
    public const string FunctionName = "ValidateJsonAgainstSchema";

    [Function(FunctionName)]
    [OpenApiOperation(operationId: FunctionName, tags: new[] { "ExecuteFunction" }, Description = "Evaluates whether a JSON instance satisfies given JSON schema.")]
    [OpenApiRequestBody("application/json", typeof(SchemaValidationRequestBody), Description = "JSON instance and a JSON schema.", Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns plain text information about whether the given JSON satisfies given JSON schema.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the error of the input.")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        var response = req.CreateResponse();
        response.Headers.Add("Content-Type", "text/plain");

        var requestBody = await req.ReadFromJsonAsync<SchemaValidationRequestBody>();
        var requestValidationResult = this.ValidateRequestBody(requestBody);
        if (!requestValidationResult.IsValid)
        {
            response.StatusCode = HttpStatusCode.BadRequest;
            response.WriteString(requestValidationResult.ValidationError!);
        }
        else
        {
            var schemaParsingResult = this.ParseSchema(requestBody!.JsonSchema!);
            var schema = schemaParsingResult.Schema;
            if (schema == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.WriteString(schemaParsingResult.ParsingError!);
            }
            else
            {
                var jsonParsingResult = this.ParseJson(requestBody.JsonInstance!);
                var json = jsonParsingResult.JsonNode;

                if (json == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.WriteString(jsonParsingResult.ParsingError!);
                }
                else
                {
                    response.StatusCode = HttpStatusCode.OK;
                    var evaluationResult = schema.Evaluate(json);

                    var responseContent = evaluationResult.IsValid
                        ? "This JSON satisfies given JSON schema."
                        : $"This JSON does not satisfy given JSON schema. Here are the errors:\n{evaluationResult.Errors}";
                    response.WriteString(responseContent);
                }
            }
        }

        return response;
    }

    private (bool IsValid, string? ValidationError) ValidateRequestBody(SchemaValidationRequestBody? body)
    {
        if (body == null)
        {
            return (false, "Request body is empty.");
        }
        else if (string.IsNullOrWhiteSpace(body.JsonInstance))
        {
            return (false, $"{nameof(body.JsonInstance)} is empty and is required.");
        }
        else if (string.IsNullOrWhiteSpace(body.JsonSchema))
        {
            return (false, $"{nameof(body.JsonSchema)} is empty and is required.");
        }

        return (true, null);
    }

    private (JsonSchema? Schema, string? ParsingError) ParseSchema(string schema)
    {
        try
        {
            return (JsonSchema.FromText(schema), null);
        }
        catch (JsonException e)
        {
            return (null, e.Message);
        }
    }

    private (JsonNode? JsonNode, string? ParsingError) ParseJson(string json)
    {
        try
        {
            return (JsonNode.Parse(json), null);
        }
        catch (JsonException e)
        {
            return (null, e.Message);
        }
    }
}
