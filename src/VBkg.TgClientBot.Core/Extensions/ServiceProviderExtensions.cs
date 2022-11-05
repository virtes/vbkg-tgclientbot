using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace VBkg.TgClientBot;

public static class ServiceProviderExtensions
{
    public static TOptions GetOptionsValue<TOptions>(this IServiceProvider serviceProvider)
        where TOptions : class
    {
        var optionsAccessor = serviceProvider.GetRequiredService<IOptions<TOptions>>();
        return optionsAccessor.Value;
    }
}