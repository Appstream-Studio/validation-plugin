using MediatR;

namespace AppStream.ValidateObject.Plugin.Requests.ValidateXmlStructureAlignment;

internal sealed record ValidateXmlStructureAlignment(string Xml1, string Xml2) : IRequest<XmlStructureAlignmentValidationResult>;
