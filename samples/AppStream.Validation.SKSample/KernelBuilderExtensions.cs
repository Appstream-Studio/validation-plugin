using AppStream.Validation.SKSample.Settings;
using Microsoft.SemanticKernel;

namespace AppStream.Validation.SKSample;

internal static class KernelBuilderExtensions
{
    internal static KernelBuilder WithChatCompletionService(this KernelBuilder kernelBuilder, KernelSettings kernelSettings)
    {
        return kernelSettings.ServiceType.ToUpperInvariant() switch
        {
            KernelSettings.ServiceTypes.AzureOpenAI => kernelBuilder.WithAzureChatCompletionService(
                deploymentName: kernelSettings.DeploymentOrModelId,
                endpoint: kernelSettings.Endpoint,
                apiKey: kernelSettings.ApiKey,
                serviceId: kernelSettings.ServiceId),

            KernelSettings.ServiceTypes.OpenAI => kernelBuilder.WithOpenAIChatCompletionService(
                modelId: kernelSettings.DeploymentOrModelId,
                apiKey: kernelSettings.ApiKey,
                orgId: kernelSettings.OrgId,
                serviceId: kernelSettings.ServiceId),

            _ => throw new ArgumentException($"Invalid service type value: {kernelSettings.ServiceType}")
        };
    }
}
