using Chabot.Proxy.RabbitMq;
using Chabot.Telegram;
using Destructurama;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using VBkg.TgClientBot;
using VBkg.TgClientBot.Listener.Configuration;
using VBkg.TgClientBot.Listener.Middlewares;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host, services) =>
    {
        services.AddOptionsFromConfiguration<WhiteListOptions>("WhiteListOptions");

        services.AddTelegramChabot((c, _) => c.Token = host.Configuration["TelegramBotOptions:Token"], c =>
        {
            c.UseTelegramPollingUpdates();

            c.UseMiddleware<WhiteListMiddleware>();
            c.UseRabbitMqListenerProxy(o
                => host.Configuration.Bind("RabbitMqProxyOptions", o));
        });
    })
    .UseSerilog((host, _, configuration) => configuration
        .ReadFrom.Configuration(host.Configuration)
        .Destructure.UsingAttributes()
        .Enrich.WithSpan()
        .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
            .WithDefaultDestructurers()
            .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() })))
    .Build();

await host.RunAsync();