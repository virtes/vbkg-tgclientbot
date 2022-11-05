using System.Globalization;
using Chabot.Message;
using TelegramMessage = Telegram.Bot.Types.Message;
using TelegramUser = Telegram.Bot.Types.User;

namespace VBkg.TgClientBot.Worker.Middlewares;

public class LocalizationMiddleware : IMiddleware<TelegramMessage, TelegramUser>
{
    private static readonly CultureInfo DefaultCulture = CultureInfo.GetCultureInfo("ru-RU");

    private readonly ILogger<LocalizationMiddleware> _logger;

    public LocalizationMiddleware(ILogger<LocalizationMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Invoke(MessageContext<TelegramMessage, TelegramUser> messageContext,
        HandleMessage<TelegramMessage, TelegramUser> next)
    {
        CultureInfo cultureInfo;

        if (messageContext.User.LanguageCode is not null)
        {
            cultureInfo = CultureInfo.GetCultureInfoByIetfLanguageTag(messageContext.User.LanguageCode);
        }
        else
        {
            _logger.LogDebug("User {UserId} has no language code, using default culture ({CultureName})",
                messageContext.User.Id, DefaultCulture.Name);
            cultureInfo = DefaultCulture;
        }

        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;

        await next(messageContext);
    }
}