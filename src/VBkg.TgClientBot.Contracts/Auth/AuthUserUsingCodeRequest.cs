using Destructurama.Attributed;
using MediatR;

namespace VBkg.TgClientBot.Contracts.Auth;

public class AuthUserUsingCodeRequest : IRequest<Either<SuccessAuth, InvalidAuthCode>>
{
    public AuthUserUsingCodeRequest(long telegramUserId, string authCode)
    {
        TelegramUserId = telegramUserId;
        AuthCode = authCode;
    }

    public long TelegramUserId { get; }

    [LogMasked(ShowLast = 2)]
    public string AuthCode { get; }
}

public record struct SuccessAuth;
public record struct InvalidAuthCode;