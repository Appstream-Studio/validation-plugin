using FluentValidation;

namespace AppStream.ValidateObject.Plugin.Requests.ValidateJsonFormat;

internal sealed class ValidateJsonFormatValidator : AbstractValidator<ValidateJsonFormat>
{
    public ValidateJsonFormatValidator()
    {
        this.RuleFor(r => r.Json).NotNull().NotEmpty();
    }
}
