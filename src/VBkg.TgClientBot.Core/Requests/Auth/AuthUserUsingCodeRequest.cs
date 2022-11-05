using Destructurama.Attributed;
using MediatR;
using OneOf;
using Success = VBkg.TgClientBot.Core.Requests.Auth.AuthUserUsingCodeRequest.Success;
using InvalidAuthCode = VBkg.TgClientBot.Core.Requests.Auth.AuthUserUsingCodeRequest.InvalidAuthCode;

namespace VBkg.TgClientBot.Core.Requests.Auth;

public class AuthUserUsingCodeRequest : IRequest<OneOf<Success, InvalidAuthCode>>
{
    public AuthUserUsingCodeRequest(long telegramUserId, string authCode)
    {
        TelegramUserId = telegramUserId;
        AuthCode = authCode;
    }

    public long TelegramUserId { get; }

    [LogMasked(ShowLast = 2)]
    public string AuthCode { get; }

    public record struct Success;
    public record struct InvalidAuthCode;
}
