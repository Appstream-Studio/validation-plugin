using MediatR;

namespace AppStream.ValidateObject.Plugin.Requests.ValidateXmlFormat;

internal sealed record ValidateXmlFormat(string Xml) : IRequest<XmlFormatValidationResult>;
