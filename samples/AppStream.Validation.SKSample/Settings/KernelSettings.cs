namespace AppStream.Validation.SKSample.Settings;

internal sealed class KernelSettings
{
    public string ServiceType { get; set; } = string.Empty;
    public string ServiceId { get; set; } = string.Empty;
    public string DeploymentOrModelId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string OrgId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;

    internal static class ServiceTypes
    {
        internal const string OpenAI = "OPENAI";
        internal const string AzureOpenAI = "AZUREOPENAI";
    }
}
