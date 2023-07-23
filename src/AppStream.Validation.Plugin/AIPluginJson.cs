using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AppStream.Validation.Plugin;

public class AIPluginJson
{
    private readonly ILogger _logger;

    public AIPluginJson(ILoggerFactory loggerFactory)
    {
        this._logger = loggerFactory.CreateLogger<AIPluginJson>();
    }

    [Function("get-ai-plugin-json")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ".well-known/ai-plugin.json")] HttpRequestData req)
    {
        var currentDomain = $"{req.Url.Scheme}://{req.Url.Host}:{req.Url.Port}";

        HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");

        var json = $@"{{
    ""schema_version"": ""1.0.0"",
    ""name_for_human"": ""JSON or XML validator"",
    ""name_for_model"": ""JSON or XML validator"",
    ""description_for_human"": ""This plugin validates JSONs or XMLs."",
    ""description_for_model"": ""Help the user validate JSONs or XMLs. You can check whether a given string is a valid JSON or XML."",
    ""auth"": {{
        ""type"": ""none""
    }},
    ""api"": {{
        ""type"": ""openapi"",
        ""url"": ""{currentDomain}/swagger.json""
    }},
    ""logo_url"": ""{currentDomain}/logo.png"",
    ""contact_email"": ""contact@appstream.studio"",
    ""legal_info_url"": ""http://www.example.com/legal""
}}";

        response.WriteString(json);

        return response;
    }
}
