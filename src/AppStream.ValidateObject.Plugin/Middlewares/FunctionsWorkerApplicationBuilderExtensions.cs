using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;

namespace AppStream.ValidateObject.Plugin.Middlewares;

internal static class FunctionsWorkerApplicationBuilderExtensions
{
    public static IFunctionsWorkerApplicationBuilder UseValidationExceptionHandling(this IFunctionsWorkerApplicationBuilder builder)
        => builder.UseMiddleware<ValidationExceptionHandlingMiddleware>();
}
