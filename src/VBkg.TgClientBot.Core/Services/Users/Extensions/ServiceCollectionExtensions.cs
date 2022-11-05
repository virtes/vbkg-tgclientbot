using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VBkg.TgClientBot.Core.Services.Users.Implementation;

namespace VBkg.TgClientBot.Core.Services.Users.Extensions;

public static class ServiceCollectionExtensions
{
    public static void TryAddUserServices(this IServiceCollection services)
    {
        services.TryAddScoped<IUserService, UserService>();
    }
}