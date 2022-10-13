using VBkg.TgClientBot.Data.Entities;

namespace VBkg.TgClientBot.Services.Users;

public interface IUserService
{
    User CreateUser(long telegramUserId, long vbkgId);

    Task<User?> GetUserByTelegramUserId(long telegramUserId);
}