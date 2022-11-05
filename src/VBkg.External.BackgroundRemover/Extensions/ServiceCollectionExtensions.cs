using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VBkg.External.BackgroundRemover.Configuration;
using VBkg.External.BackgroundRemover.Implementation;

namespace VBkg.External.BackgroundRemover.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddBackgroundRemoverClient(this IServiceCollection services,
        Action<BackgroundRemoverClientOptions> configureOptions)
    {
        services.AddOptions<BackgroundRemoverClientOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient<IBackgroundRemoverClient, BackgroundRemoverClient>((sp, hc) =>
        {
            var optionsAccessor = sp.GetRequiredService<IOptions<BackgroundRemoverClientOptions>>();
            var options = optionsAccessor.Value;

            hc.Timeout = TimeSpan.FromMinutes(3);
            hc.BaseAddress = new Uri(options.Host);
        });
    }
}