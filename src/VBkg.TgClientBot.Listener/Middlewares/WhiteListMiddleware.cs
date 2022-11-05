using Chabot.Message;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using VBkg.TgClientBot.Listener.Configuration;

namespace VBkg.TgClientBot.Listener.Middlewares;

public class WhiteListMiddleware : IMiddleware<Message, User>
{
    private readonly ILogger<WhiteListMiddleware> _logger;
    private readonly IOptions<WhiteListOptions> _optionsAccessor;

    public WhiteListMiddleware(
        ILogger<WhiteListMiddleware> logger,
        IOptions<WhiteListOptions> optionsAccessor)
    {
        _logger = logger;
        _optionsAccessor = optionsAccessor;
    }

    public async Task Invoke(MessageContext<Message, User> messageContext,
        HandleMessage<Message, User> next)
    {
        var options = _optionsAccessor.Value;
        if (options.AllowedUserIds is not null)
        {
            if (!options.AllowedUserIds.Contains(messageContext.User.Id))
            {
                _logger.LogInformation("White list doesnt contain " +
                                       "telegram user {TelegramUserId} ({WhiteListCount})",
                    messageContext.User.Id, options.AllowedUserIds.Count);
                return;
            }
        }

        await next(messageContext);
    }
}