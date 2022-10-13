using Chabot;
using Chabot.Proxy.RabbitMq;
using Chabot.State;
using Chabot.Telegram;
using Destructurama;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using VBkg.TgClientBot.Core;
using VBkg.TgClientBot.Data;
using VBkg.TgClientBot.Data.Chabot;
using VBkg.TgClientBot.Proto.Server;
using VBkg.TgClientBot.Services.Users.Extensions;
using VBkg.TgClientBot.Worker.Middlewares;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host, services) =>
    {
        services.AddScoped<IStateStorage<long, string>, DbContextStateStorage>();
        services.AddTgChabot(c => c.Token = host.Configuration["TelegramBotOptions:Token"], c =>
        {
            c.UseRabbitMqWorkerProxy(o => o.BindConfiguration("RabbitMqProxyOptions"));

            c.UseMiddleware<LocalizationMiddleware>();
            c.UseState(s => s
                .UseSystemTextJsonSerializer());

            c.UseCommands();
        });

        services.AddDbContext<AppDbContext>(c
            => c.UseNpgsql(host.Configuration.GetConnectionString(nameof(AppDbContext))));

        services.AddUsersGrpcClient(
            host: host.Configuration["ServerGrpcApi:Host"],
            apiKey: host.Configuration["ServerGrpcApi:ApiKey"]);

        services.AddMediatR(typeof(CoreAssemblyMarker));
        services.AddLocalization();
        services.AddSingleton<LocalizationMiddleware>();

        services.TryAddUserServices();
    })
    .UseSerilog((host, _, configuration) => configuration
        .ReadFrom.Configuration(host.Configuration)
        .Destructure.UsingAttributes()
        .Enrich.WithSpan()
        .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
            .WithDefaultDestructurers()
            .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() })))
    .Build();

host.MigrateAppDbContext();
await host.RunAsync();