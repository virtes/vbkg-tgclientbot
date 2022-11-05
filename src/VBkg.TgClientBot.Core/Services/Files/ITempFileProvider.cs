namespace VBkg.TgClientBot.Core.Services.Files;

public interface ITempFileProvider
{
    Stream CreateTempFile();
}