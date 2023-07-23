using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace AppStream.Validation.Plugin.OpenApi;

internal static class OpenApiServiceCollectionExtensions
{
    public static IServiceCollection AddOpenApiConfigurationOptions(this IServiceCollection services)
    {
        return services.AddSingleton<IOpenApiConfigurationOptions>(_ =>
        {
            return new OpenApiConfigurationOptions
            {
                Info = new OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = "AI validation plugin",
                    Description = "[ChatGPT](https://openai.com/blog/chatgpt-plugins)/[Semantic Kernel](https://learn.microsoft.com/en-us/semantic-kernel/ai-orchestration/plugins?tabs=Csharp) plugin for validating JSONs or XMLs.",
                    Contact = new OpenApiContact()
                    {
                        Name = "Enquiry",
                        Email = "contact@appstream.studio",
                        Url = new Uri("https://github.com/Azure/azure-functions-openapi-extension/issues"),
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "Apache-2.0",
                        Url = new Uri("https://opensource.org/license/apache-2-0/"),
                    }
                },
                Servers = DefaultOpenApiConfigurationOptions.GetHostNames(),
                OpenApiVersion = OpenApiVersionType.V2,
                IncludeRequestingHostName = true,
                ForceHttps = false,
                ForceHttp = false,
            };
        });
    }
}
