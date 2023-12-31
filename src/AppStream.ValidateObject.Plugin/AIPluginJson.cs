﻿using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Options;

namespace AppStream.ValidateObject.Plugin;

public class AIPluginJson
{
    private readonly AIPluginOptions _options;

    public AIPluginJson(IOptions<AIPluginOptions> options)
    {
        this._options = options.Value;
    }

    [Function("GetAIPluginJson")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ".well-known/ai-plugin.json")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");

        // serialize app settings to json using System.Text.Json
        var json = System.Text.Json.JsonSerializer.Serialize(this._options);

        // replace {url} with the current domain
        var currentDomain = $"{req.Url.Scheme}://{req.Url.Host}:{req.Url.Port}";
        json = json.Replace("{url}", currentDomain, StringComparison.OrdinalIgnoreCase);

        response.WriteString(json);

        return response;
    }
}
