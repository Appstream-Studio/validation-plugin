using System.Xml;
using System.Xml.Schema;
using MediatR;

namespace AppStream.ValidateObject.Plugin.Requests.ValidateXmlStructureAlignment;

internal sealed class ValidateXmlStructureAlignmentHandler : IRequestHandler<ValidateXmlStructureAlignment, XmlStructureAlignmentValidationResult>
{
    public Task<XmlStructureAlignmentValidationResult> Handle(ValidateXmlStructureAlignment request, CancellationToken cancellationToken)
    {
        var validationErrors = new List<string>();

        using var reader1 = XmlReader.Create(new StringReader(request.Xml1));
        var schema = new XmlSchemaInference();
        var schemaSet = schema.InferSchema(reader1);

        var document = new XmlDocument();
        document.Schemas.Add(schemaSet);
        document.LoadXml(request.Xml2);

        document.Validate((sender, args) =>
        {
            validationErrors.Add(args.Message);
        });

        return Task.FromResult(new XmlStructureAlignmentValidationResult(validationErrors.Count == 0, validationErrors.ToArray()));
    }
}
