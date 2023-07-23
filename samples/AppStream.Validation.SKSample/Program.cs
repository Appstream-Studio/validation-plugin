using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;

var configRoot = new ConfigurationBuilder()
    .AddJsonFile("appsettings.local.json")
    .Build();

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.SetMinimumLevel(LogLevel.Trace);

    builder.AddFilter("Microsoft", LogLevel.Warning);
    builder.AddFilter("System", LogLevel.Warning);

    builder.AddConsole();
});

// load config

var kernel = new KernelBuilder()
    .WithLogger(loggerFactory.CreateLogger<Kernel>())
    .WithOpenAIChatCompletionService("modelId", "apiKey")
    .Build();

const string pluginManifestUrl = "http://localhost:7071/.well-known/ai-plugin.json";
var isJsonValidPlugin = await kernel.ImportChatGptPluginSkillFromUrlAsync("is-json-valid", new Uri(pluginManifestUrl));

var planner = new StepwisePlanner(kernel);
var question = "I have $2130.23. How much would I have after it grew by 24% and after I spent $5 on a latte?";
var plan = planner.CreatePlan(question);
var result = await plan.InvokeAsync(kernel.CreateNewContext());
