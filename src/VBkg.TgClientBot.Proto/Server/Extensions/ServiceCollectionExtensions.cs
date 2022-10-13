using Microsoft.Extensions.DependencyInjection;
using VBkg.TgClientBot.Proto.Server.Implementation;

// ReSharper disable once CheckNamespace
namespace VBkg.TgClientBot.Proto.Server;

public static class ServiceCollectionExtensions
{
    public static void AddUsersGrpcClient(this IServiceCollection services,
        string host, string apiKey)
    {
        if (string.IsNullOrEmpty(host))
            throw new ArgumentException("Host is empty", nameof(host));

        if (string.IsNullOrEmpty(apiKey))
            throw new ArgumentException("Api key is empty", nameof(apiKey));

        services
            .AddGrpcClient<Users.UsersClient>(o =>
            {
                o.Address = new Uri(host);
            })
            .AddInterceptor(() => new AuthorizationHeaderInterceptor(apiKey));
    }
}