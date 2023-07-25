using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AppStream.ValidateObject.Plugin.MediatR;

internal static class MediatRServiceCollectionExtensions
{
    public static IServiceCollection AddConfiguredMediatR(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services
            .AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblies(assemblies);
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            })
            .AddValidators(assemblies);
    }

    private static IServiceCollection AddValidators(this IServiceCollection services, params Assembly[] assemblies)
    {
        var types = assemblies.SelectMany(a => a.GetTypes());
        var scanner = new AssemblyScanner(types);

        foreach (var pair in scanner)
        {
            services.AddTransient(pair.InterfaceType, pair.ValidatorType);
        }

        return services;
    }
}
