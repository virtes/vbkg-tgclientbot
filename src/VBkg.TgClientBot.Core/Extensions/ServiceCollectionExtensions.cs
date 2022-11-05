using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace VBkg.TgClientBot;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOptionsFromConfiguration<TOptions>(
        this IServiceCollection services, string sectionPath) where TOptions : class
    {
        services.AddOptions<TOptions>()
            .BindConfiguration(sectionPath)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}