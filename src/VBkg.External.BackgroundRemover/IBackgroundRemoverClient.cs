using VBkg.External.BackgroundRemover.Dto;

namespace VBkg.External.BackgroundRemover;

public interface IBackgroundRemoverClient
{
    Task<RemoveBackgroundResponseDto> RemoveBackground(
        RemoveBackgroundRequestDto request, Stream resultStream);
}