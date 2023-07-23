using AppStream.Validation.Plugin.OpenApi;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
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
            .ConfigureServices(services => services.AddOpenApiConfigurationOptions())
            .Build();

        host.Run();
    }
}
