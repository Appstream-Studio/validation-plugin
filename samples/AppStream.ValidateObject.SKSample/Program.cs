using System.Reflection;
using System.Text;
using AppStream.ValidateObject.SKSample;
using AppStream.ValidateObject.SKSample.Settings;
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
            builder.AddConsole();
        });

        var kernel = new KernelBuilder()
            .WithLogger(loggerFactory.CreateLogger<Kernel>())
            .WithChatCompletionService(appSettings.Kernel)
            .Build();

        await kernel.ImportChatGptPluginSkillFromUrlAsync("ValidateObject", new Uri(appSettings.AIPlugin.ManifestUrl));
        var planner = new SequentialPlanner(kernel);

        var logger = loggerFactory.CreateLogger<Program>();
        await RunExample("Is this a valid json? { \"foo\"dd: 1 }", kernel, planner, logger);
        await RunExample("Is this a valid json? { \"foo\": 1 }", kernel, planner, logger);

        var jsonSchema = @"{
  ""properties"":{
    ""myProperty"":{
      ""type"":""string"",
      ""minLength"":10
    }
  },
  ""required"":[""myProperty""]
}";

        await RunExample($"Does this json satisfy given schema? json: {{}}, schema: {jsonSchema}", kernel, planner, logger);
        await RunExample($"Does this json satisfy given schema? json: {{\"myProperty\":false}}, schema: {jsonSchema}", kernel, planner, logger);
        await RunExample($"Does this json satisfy given schema? json: {{\"myProperty\":\"some string\"}}, schema: {jsonSchema}", kernel, planner, logger);
        await RunExample($"Does this json satisfy given schema? json: {{\"otherProperty\":35.4}}, schema: {jsonSchema}", kernel, planner, logger);
        await RunExample($"Does this json satisfy given schema? json: \"nonObject\", schema: {jsonSchema}", kernel, planner, logger);
    }

    private static async Task RunExample(string question, IKernel kernel, SequentialPlanner planner, ILogger<Program> logger)
    {
        var plan = await planner.CreatePlanAsync(question);
        var sb = new StringBuilder("Plan:\n");
        for (var i = 0; i < plan.Steps.Count; i++)
        {
            sb.AppendLine($"{i + 1}. {plan.Steps[i].Name}");
        }
        logger.LogInformation(sb.ToString());

        var result = await plan.InvokeAsync(kernel.CreateNewContext());
        logger.LogInformation("Response: " + result);
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
