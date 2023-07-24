using AppStream.Validation.Plugin;
using AppStream.Validation.Plugin.OpenApi;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal sealed class Program
{
    private static void Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults(worker =>
            {
                // Required by Microsoft.Azure.Functions.Worker.Extensions.OpenApi
                // https://github.com/Azure/azure-functions-openapi-extension/blob/main/docs/enable-open-api-endpoints-out-of-proc.md#enable-openapi-document
                worker.UseNewtonsoftJson();
            })
            .ConfigureAppConfiguration(builder => builder.AddJsonFile("appsettings.json", optional: false))
            .ConfigureServices(services =>
            {
                services.AddOptions<AIPluginOptions>().BindConfiguration(AIPluginOptions.AIPlugin).ValidateDataAnnotations().ValidateOnStart();
                services.AddOpenApiConfigurationOptions();
            })
            .Build();

        host.Run();
    }
}
