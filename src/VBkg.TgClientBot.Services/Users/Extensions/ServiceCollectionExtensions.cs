using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VBkg.TgClientBot.Services.Users.Implementation;

namespace VBkg.TgClientBot.Services.Users.Extensions;

public static class ServiceCollectionExtensions
{
    public static void TryAddUserServices(this IServiceCollection services)
    {
        services.TryAddScoped<IUserService, UserService>();
    }
}