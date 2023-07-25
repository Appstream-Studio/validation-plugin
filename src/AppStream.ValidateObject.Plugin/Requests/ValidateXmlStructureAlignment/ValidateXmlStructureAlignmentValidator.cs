using System.Xml;
using FluentValidation;

namespace AppStream.ValidateObject.Plugin.Requests.ValidateXmlStructureAlignment;

internal sealed class ValidateXmlStructureAlignmentValidator : AbstractValidator<ValidateXmlStructureAlignment>
{
    public ValidateXmlStructureAlignmentValidator()
    {
        this.RuleFor(r => r.Xml1).NotNull().NotEmpty()
            .Must(this.IsValidXml)
            .WithMessage("'{PropertyName}' must be a valid XML ({ErrorMessage}).");

        this.RuleFor(r => r.Xml2).NotNull().NotEmpty()
            .Must(this.IsValidXml)
            .WithMessage("'{PropertyName}' must be a valid XML ({ErrorMessage}).");
    }

    private bool IsValidXml(ValidateXmlStructureAlignment root, string xmlInstance, ValidationContext<ValidateXmlStructureAlignment> context)
    {
        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlInstance);
            return true;
        }
        catch (XmlException e)
        {
            context.MessageFormatter.AppendArgument("ErrorMessage", e.Message);
            return false;
        }
    }
}
