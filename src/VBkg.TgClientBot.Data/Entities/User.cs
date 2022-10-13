namespace VBkg.TgClientBot.Data.Entities;

public class User
{
    public Guid Id { get; protected set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? RemovedAt { get; set; }

    public long TelegramUserId { get; set; }

    public long VbkgId { get; set; } = default!;
}