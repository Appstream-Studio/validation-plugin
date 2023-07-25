using MediatR;

namespace AppStream.ValidateObject.Plugin.Requests.EvaluateXmlSchema;

internal sealed record EvaluateXmlSchema(string XmlInstance, string XmlSchema) : IRequest<XmlSchemaEvaluationResult>;
