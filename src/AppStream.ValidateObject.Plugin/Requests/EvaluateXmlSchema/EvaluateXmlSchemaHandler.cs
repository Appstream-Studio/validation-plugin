using System.Xml;
using System.Xml.Schema;
using MediatR;

namespace AppStream.ValidateObject.Plugin.Requests.EvaluateXmlSchema;

internal sealed class EvaluateXmlSchemaHandler : IRequestHandler<EvaluateXmlSchema, XmlSchemaEvaluationResult>
{
    public Task<XmlSchemaEvaluationResult> Handle(EvaluateXmlSchema request, CancellationToken cancellationToken)
    {
        var schema = XmlSchema.Read(new StringReader(request.XmlSchema), null!)!;
        var validationErrors = new List<string>();

        var document = new XmlDocument();
        document.Schemas.Add(schema);
        document.LoadXml(request.XmlInstance);

        document.Validate((sender, args) =>
        {
            validationErrors.Add(args.Message);
        });

        return Task.FromResult(new XmlSchemaEvaluationResult(validationErrors.Count == 0, validationErrors.ToArray()));
    }
}
