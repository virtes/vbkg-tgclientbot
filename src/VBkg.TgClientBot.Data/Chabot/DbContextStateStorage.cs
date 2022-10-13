using Chabot.State;
using UserState = VBkg.TgClientBot.Data.Entities.UserState;

namespace VBkg.TgClientBot.Data.Chabot;

public class DbContextStateStorage : IStateStorage<long, string>
{
    private readonly AppDbContext _dbContext;

    public DbContextStateStorage(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask WriteState(long userId, string state)
    {
        var userState = await _dbContext.UserStates.FindAsync(userId);
        if (userState is null)
        {
            userState = new UserState
            {
                TelegramUserId = userId
            };
            _dbContext.UserStates.Add(userState);
        }

        userState.Value = state;
        userState.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async ValueTask<string?> ReadState(long userId)
    {
        var userState = await _dbContext.UserStates.FindAsync(userId);

        return userState?.Value;
    }
}