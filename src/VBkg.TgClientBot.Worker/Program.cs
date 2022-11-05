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
using VBkg.External.BackgroundRemover.Extensions;
using VBkg.TgClientBot;
using VBkg.TgClientBot.Core;
using VBkg.TgClientBot.Core.Services.Files.Extensions;
using VBkg.TgClientBot.Core.Services.Users.Extensions;
using VBkg.TgClientBot.Data;
using VBkg.TgClientBot.Proto.Server;
using VBkg.TgClientBot.Worker.Chabot;
using VBkg.TgClientBot.Worker.Configuration;
using VBkg.TgClientBot.Worker.Middlewares;
using VBkg.TgClientBot.Worker.States;
using TelegramMessage = Telegram.Bot.Types.Message;
using TelegramUser = Telegram.Bot.Types.User;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((host, services) =>
    {
        services.AddScoped<IStateStorage<TelegramMessage, TelegramUser, string>, DbContextStateStorage>();
        services.AddSingleton<IDefaultStateFactory<TelegramMessage, TelegramUser>, TgClientBotDefaultStateFactory>();
        services.AddSingleton<LocalizationMiddleware>();
        services.AddTelegramChabot((c, sp) => c.Token = sp.GetOptionsValue<TelegramBotOptions>().Token, c =>
        {
            c.UseRabbitMqWorkerProxy(o
                => host.Configuration.Bind("RabbitMqProxyOptions", o));

            c.UseState(s => s
                .UseSystemTextJsonSerializer());

            c.UseMiddleware<LocalizationMiddleware>();
            c.UseCommands();
        });

        services.AddDbContext<AppDbContext>(c
            => c.UseNpgsql(host.Configuration.GetConnectionString(nameof(AppDbContext))));

        services.AddBackgroundRemoverClient(o
            => host.Configuration.Bind("BackgroundRemoverClientOptions", o));

        services.AddServerGrpcClient(o
            => host.Configuration.Bind("ServerGrpcClientOptions", o));

        services.AddOptionsFromConfiguration<TelegramBotOptions>("TelegramBotOptions");

        services.AddMediatR(typeof(CoreAssemblyMarker));
        services.AddLocalization();

        services.TryAddFileServices();
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