using Chabot.Proxy.RabbitMq;
using Chabot.Telegram;
using Destructurama;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host, services) =>
    {
        services.AddTgChabot(c => c.Token = host.Configuration["TelegramBotOptions:Token"], c =>
        {
            c.UseTelegramPollingUpdates();

            c.UseRabbitMqListenerProxy(o => o.BindConfiguration("RabbitMqProxyOptions"));
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