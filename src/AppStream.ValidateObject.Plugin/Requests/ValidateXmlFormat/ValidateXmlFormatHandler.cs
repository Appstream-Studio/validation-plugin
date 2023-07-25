using System.Xml;
using MediatR;

namespace AppStream.ValidateObject.Plugin.Requests.ValidateXmlFormat;

internal sealed class ValidateXmlFormatHandler : IRequestHandler<ValidateXmlFormat, XmlFormatValidationResult>
{
    public Task<XmlFormatValidationResult> Handle(ValidateXmlFormat request, CancellationToken cancellationToken)
    {
        XmlFormatValidationResult result;
        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(request.Xml);
            result = new XmlFormatValidationResult(true, null);
        }
        catch (XmlException e)
        {
            result = new XmlFormatValidationResult(false, e.Message);
        }

        return Task.FromResult(result);
    }
}
