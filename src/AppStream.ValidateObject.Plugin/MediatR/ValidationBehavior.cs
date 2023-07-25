using FluentValidation;
using MediatR;

namespace AppStream.ValidateObject.Plugin.MediatR;

internal sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        this._validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!this._validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var errorsDictionary = this._validators
            .Select(async x => await x.ValidateAsync(context, cancellationToken))
            .Select(t => t.Result)
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .ToArray();

        if (errorsDictionary.Any())
        {
            throw new ValidationException(errorsDictionary);
        }

        return await next();
    }
}
