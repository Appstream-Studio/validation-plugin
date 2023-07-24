using System.Reflection;
using AppStream.Validation.SKSample;
using AppStream.Validation.SKSample.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;

internal sealed class Program
{
    private static async Task Main(string[] args)
    {
        var appSettings = LoadConfiguration();

        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddFilter("Microsoft", LogLevel.Information);
            builder.AddConsole();
        });

        var kernel = new KernelBuilder()
            .WithLogger(loggerFactory.CreateLogger<Kernel>())
            .WithChatCompletionService(appSettings.Kernel)
            .Build();

        var isJsonValidPlugin = await kernel.ImportChatGptPluginSkillFromUrlAsync("is_json_valid", new Uri(appSettings.AIPlugin.ManifestUrl));
        var planner = new SequentialPlanner(kernel);

        var logger = loggerFactory.CreateLogger<Program>();
        await RunExample("Is this a valid json? { \"foo\"dd: 1 }", kernel, planner, logger);
        await RunExample("Is this a valid json? { \"foo\": 1 }", kernel, planner, logger);
    }

    private static async Task RunExample(string question, IKernel kernel, SequentialPlanner planner, ILogger<Program> logger)
    {
        logger.LogInformation("Question: " + question);

        var plan = await planner.CreatePlanAsync(question);
        var result = await plan.InvokeAsync(kernel.CreateNewContext());

        logger.LogInformation("Result: " + result);
    }

    private static AppSettings LoadConfiguration()
    {
        const string configFileName = "appsettings.local.json";
        const string exampleConfigFileName = "appsettings.local.json.example";
        var configRoot = new ConfigurationBuilder()
            .AddJsonFile(configFileName, optional: false)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: false)
            .Build();

        var appSettings = configRoot.Get<AppSettings>();
        if (appSettings == null || appSettings.Kernel == null || appSettings.AIPlugin == null)
        {
            throw new InvalidDataException(
                $"Invalid app settings in {configFileName}. Please provide settings based on the example in {exampleConfigFileName}.");
        }

        return appSettings;
    }
}
