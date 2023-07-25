using FluentValidation;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace AppStream.ValidateObject.Plugin.Middlewares;

internal sealed class ValidationExceptionHandlingMiddleware : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (AggregateException e) when (e.InnerException is ValidationException validationException)
        {
            var request = await context.GetHttpRequestDataAsync();
            if (request != null)
            {
                var response = request.CreateResponse();

                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.Headers.Add("Content-Type", "text/plain");
                response.WriteString(validationException.Message);

                context.GetInvocationResult().Value = response;
            }
        }
    }
}
