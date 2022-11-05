using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VBkg.External.Server.Configuration;
using VBkg.External.Server.Implementation;
using VBkg.Server.Proto;

// ReSharper disable once CheckNamespace
namespace VBkg.TgClientBot.Proto.Server;

public static class ServiceCollectionExtensions
{
    public static void AddServerGrpcClient(this IServiceCollection services,
        Action<ServerGrpcClientOptions> configureOptions)
    {
        services.AddOptions<ServerGrpcClientOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddGrpcClient<Users.UsersClient>((sp, o) =>
            {
                var optionsAccessor = sp.GetRequiredService<IOptions<ServerGrpcClientOptions>>();
                o.Address = new Uri(optionsAccessor.Value.Host);
            })
            .AddInterceptor(sp =>
            {
                var optionsAccessor = sp.GetRequiredService<IOptions<ServerGrpcClientOptions>>();
                return new AuthorizationHeaderInterceptor(optionsAccessor.Value.ApiKey);
            });
    }
}