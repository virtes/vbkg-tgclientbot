using Microsoft.Extensions.Logging;

namespace VBkg.TgClientBot.Core.Services.Files.Implementation;

public class TempFileProvider : ITempFileProvider
{
    private readonly ILogger<TempFileProvider> _logger;

    public TempFileProvider(ILogger<TempFileProvider> logger)
    {
        _logger = logger;
    }

    public Stream CreateTempFile()
    {
        var tempFilePath = System.IO.Path.GetTempFileName();
        _logger.LogTrace("Using {TempFilePath} path to create temp file", tempFilePath);

        var result = File.Create(tempFilePath, 4096, FileOptions.DeleteOnClose);
        _logger.LogDebug("Temp file created in {TempFilePath}", tempFilePath);

        return result;
    }
}