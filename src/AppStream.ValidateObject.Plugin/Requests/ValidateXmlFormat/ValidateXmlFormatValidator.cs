using FluentValidation;

namespace AppStream.ValidateObject.Plugin.Requests.ValidateXmlFormat;

internal sealed class ValidateXmlFormatValidator : AbstractValidator<ValidateXmlFormat>
{
    public ValidateXmlFormatValidator()
    {
        this.RuleFor(r => r.Xml).NotNull().NotEmpty();
    }
}
