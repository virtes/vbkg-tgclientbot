namespace VBkg.TgClientBot.Data.Entities;

public class UserState
{
    public long TelegramUserId { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string Value { get; set; } = default!;
}