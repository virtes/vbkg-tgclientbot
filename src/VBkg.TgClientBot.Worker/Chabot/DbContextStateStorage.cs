using Chabot.State;
using VBkg.TgClientBot.Data;
using UserState = VBkg.TgClientBot.Data.Entities.UserState;
using TelegramMessage = Telegram.Bot.Types.Message;
using TelegramUser = Telegram.Bot.Types.User;

namespace VBkg.TgClientBot.Worker.Chabot;

public class DbContextStateStorage : IStateStorage<TelegramMessage, TelegramUser, string>
{
    private readonly AppDbContext _dbContext;

    public DbContextStateStorage(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask WriteState(TelegramMessage message, TelegramUser user, string state)
    {
        var userState = await _dbContext.UserStates.FindAsync(user.Id, message.Chat.Id);
        if (userState is null)
        {
            userState = new UserState
            {
                TelegramUserId = user.Id,
                TelegramChatId = message.Chat.Id
            };
            _dbContext.UserStates.Add(userState);
        }

        userState.Value = state;
        userState.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async ValueTask<string?> ReadState(TelegramMessage message, TelegramUser user)
    {
        var userState = await _dbContext.UserStates.FindAsync(user.Id, message.Chat.Id);

        return userState?.Value;
    }
}