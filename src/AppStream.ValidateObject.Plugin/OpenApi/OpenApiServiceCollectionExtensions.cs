using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace AppStream.ValidateObject.Plugin.OpenApi;

internal static class OpenApiServiceCollectionExtensions
{
    public static IServiceCollection AddOpenApiConfigurationOptions(this IServiceCollection services)
    {
        return services.AddSingleton<IOpenApiConfigurationOptions>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AIPluginOptions>>().Value;

            return new OpenApiConfigurationOptions
            {
                Info = new OpenApiInfo
                {
                    Version = options.SchemaVersion,
                    Title = options.NameForHuman,
                    Description = options.DescriptionForHuman,
                    Contact = new OpenApiContact()
                    {
                        Name = "Enquiry",
                        Email = options.ContactEmail,
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
