using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace AppStream.ValidateObject.Plugin.Json.SchemaValidation;

public class SchemaValidation
{
    public const string FunctionName = "ValidateJsonAgainstSchema";

    [Function(FunctionName)]
    [OpenApiOperation(operationId: FunctionName, tags: new[] { "ExecuteFunction" }, Description = "When provided both JSON object to verify and JSON schema to verify against - checks if the object satisfies the given schema.")]
    [OpenApiParameter(name: "jsonInstance", Description = "escaped JSON object to be validated against schema", Type = typeof(string), In = ParameterLocation.Query, Required = true)]
    [OpenApiParameter(name: "jsonSchema", Description = "escaped JSON schema to be used to validate the object", Type = typeof(string), In = ParameterLocation.Query, Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Returns plain text information about whether the given JSON satisfies given JSON schema.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Returns the error of the input.")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req, ILogger logger)
    {
        var response = req.CreateResponse();
        response.Headers.Add("Content-Type", "text/plain");


        var jsonInstance = req.Query["jsonInstance"];
        if (jsonInstance == null)
        {
            response.StatusCode = HttpStatusCode.BadRequest;
            response.WriteString("jsonInstance parameter is missing from query.");
            return response;
        }

        var jsonSchema = req.Query["jsonSchema"];
        if (jsonSchema == null)
        {
            response.StatusCode = HttpStatusCode.BadRequest;
            response.WriteString("jsonSchema parameter is missing from query.");
            return response;
        }


        var schemaParsingResult = this.ParseSchema(jsonSchema);
        var schema = schemaParsingResult.Schema;
        if (schema == null)
        {
            response.StatusCode = HttpStatusCode.BadRequest;
            response.WriteString(schemaParsingResult.ParsingError!);
        }
        else
        {
            var jsonParsingResult = this.ParseJson(jsonInstance!);
            var json = jsonParsingResult.JsonNode;

            if (json == null)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.WriteString(jsonParsingResult.ParsingError!);
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;

                try
                {
                    var evaluationResult = schema.Evaluate(json);

                    var responseContent = evaluationResult.IsValid
                        ? "This JSON satisfies given JSON schema."
                        : "This JSON does not satisfy given JSON schema";

                    if (evaluationResult.Errors?.Count() > 0)
                    {
                        responseContent += $"Here are the errors:\n{evaluationResult.Errors}";
                    }
                }
                catch (JsonSchemaException ex)
                {
                    response.WriteString($"There was a problem with schema validation: {ex.Message}");
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
