using VBkg.TgClientBot.Data.Entities;

namespace VBkg.TgClientBot.Core.Services.Users;

public interface IUserService
{
    User CreateUser(long telegramUserId, long vbkgUserId, string vbkgUserToken);

    Task<User?> GetUserByTelegramUserId(long telegramUserId);
}