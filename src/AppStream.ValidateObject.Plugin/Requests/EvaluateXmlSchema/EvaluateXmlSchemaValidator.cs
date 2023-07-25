using System.Xml;
using System.Xml.Schema;
using FluentValidation;

namespace AppStream.ValidateObject.Plugin.Requests.EvaluateXmlSchema;

internal sealed class EvaluateXmlSchemaValidator : AbstractValidator<EvaluateXmlSchema>
{
    public EvaluateXmlSchemaValidator()
    {
        this.RuleFor(r => r.XmlInstance).NotNull().NotEmpty()
            .Must(this.IsValidXml)
            .WithMessage("'{PropertyName}' must be a valid XML ({ErrorMessage}).");

        this.RuleFor(r => r.XmlSchema).NotNull().NotEmpty()
            .Must(this.IsValidXmlSchema)
            .WithMessage("'{PropertyName}' must be a valid XML schema ({ErrorMessage}).");
    }

    private bool IsValidXml(EvaluateXmlSchema root, string xmlInstance, ValidationContext<EvaluateXmlSchema> context)
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

    private bool IsValidXmlSchema(EvaluateXmlSchema root, string xmlSchema, ValidationContext<EvaluateXmlSchema> context)
    {
        try
        {
            var schemaSet = new XmlSchemaSet();
            var schema = XmlSchema.Read(new StringReader(xmlSchema), null!)!;

            schemaSet.Add(schema);
            schemaSet.Compile();

            return true;
        }
        catch (XmlException e)
        {
            context.MessageFormatter.AppendArgument("ErrorMessage", e.Message);
            return false;
        }
    }
}
