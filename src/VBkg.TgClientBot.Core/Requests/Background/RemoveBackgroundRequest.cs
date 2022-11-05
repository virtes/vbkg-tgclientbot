using Destructurama.Attributed;
using MediatR;
using OneOf;
using SuccessResult = VBkg.TgClientBot.Core.Requests.Background.RemoveBackgroundRequest.SuccessResult;
using Error = VBkg.TgClientBot.Core.Requests.Background.RemoveBackgroundRequest.Error;

namespace VBkg.TgClientBot.Core.Requests.Background;

public class RemoveBackgroundRequest : IRequest<OneOf<SuccessResult, Error>>
{
    public RemoveBackgroundRequest(long telegramUserId,
        string imageUrl, string fileName, Stream resultStream)
    {
        TelegramUserId = telegramUserId;
        ImageUrl = imageUrl;
        FileName = fileName;
        ResultStream = resultStream;
    }

    public long TelegramUserId { get; }

    [LogMasked(ShowFirst = 10, ShowLast = 5)]
    public string ImageUrl { get; }

    public string FileName { get; }

    [NotLogged]
    public Stream ResultStream { get; }

    public record struct SuccessResult(string FileName);

    public enum Error
    {
        NotRegisteredInTelegramClientBot = 0,
        InternalError = 1,
        InsufficientBalance = 2,
        BackgroundNotFound = 3,
        UserNotFound = 4,
        FileTooLarge = 5,
        ResolutionTooHigh = 6,
        InvalidFileType = 7
    }
}