using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VBkg.TgClientBot.Data;
using VBkg.TgClientBot.Data.Entities;

namespace VBkg.TgClientBot.Core.Services.Users.Implementation;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext dbContext,
        ILogger<UserService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public User CreateUser(long telegramUserId, long vbkgUserId, string vbkgUserToken)
    {
        _logger.LogTrace("<CreateUser>: {TelegramUserId}, {VbkgId}",
            telegramUserId, vbkgUserId);

        var user = new User
        {
            CreatedAt = DateTime.UtcNow,
            TelegramUserId = telegramUserId,
            VbkgUserId = vbkgUserId,
            VbkgUserToken = vbkgUserToken
        };
        _dbContext.Users.Add(user);

        _logger.LogDebug("User {TelegramUserId} created ({@User})", telegramUserId, user);

        return user;
    }

    public async Task<User?> GetUserByTelegramUserId(long telegramUserId)
    {
        _logger.LogTrace("<GetUserByTelegramId>: {TelegramUserId}", telegramUserId);

        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.TelegramUserId == telegramUserId
                                      && u.RemovedAt == null);
    }
}