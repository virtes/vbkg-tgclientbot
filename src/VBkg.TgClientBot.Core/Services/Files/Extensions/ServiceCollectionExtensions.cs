using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VBkg.TgClientBot.Core.Services.Files.Implementation;

namespace VBkg.TgClientBot.Core.Services.Files.Extensions;

public static class ServiceCollectionExtensions
{
    public static void TryAddFileServices(this IServiceCollection services)
    {
        services.TryAddSingleton<ITempFileProvider, TempFileProvider>();
    }
}