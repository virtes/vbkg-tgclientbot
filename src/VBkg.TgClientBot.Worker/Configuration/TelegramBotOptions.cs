using System.ComponentModel.DataAnnotations;

namespace VBkg.TgClientBot.Worker.Configuration;

public class TelegramBotOptions
{
    [Required(AllowEmptyStrings = false)]
    public string Token { get; set; } = default!;

    [Required(AllowEmptyStrings = false)]
    public string Username { get; set; } = default!;
}