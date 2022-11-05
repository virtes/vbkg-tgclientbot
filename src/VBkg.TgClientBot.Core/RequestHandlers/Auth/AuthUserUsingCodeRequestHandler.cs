using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;
using VBkg.Server.Proto;
using VBkg.TgClientBot.Core.Requests.Auth;
using VBkg.TgClientBot.Core.Services.Users;
using VBkg.TgClientBot.Data;
using Success = VBkg.TgClientBot.Core.Requests.Auth.AuthUserUsingCodeRequest.Success;
using InvalidAuthCode = VBkg.TgClientBot.Core.Requests.Auth.AuthUserUsingCodeRequest.InvalidAuthCode;

namespace VBkg.TgClientBot.Core.RequestHandlers.Auth;

public class AuthUserUsingCodeRequestHandler
    : IRequestHandler<AuthUserUsingCodeRequest, OneOf<Success, InvalidAuthCode>>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<AuthUserUsingCodeRequestHandler> _logger;
    private readonly IUserService _userService;
    private readonly Users.UsersClient _usersClient;

    public AuthUserUsingCodeRequestHandler(
        AppDbContext dbContext,
        ILogger<AuthUserUsingCodeRequestHandler> logger,
        IUserService userService,
        Users.UsersClient usersClient)
    {
        _dbContext = dbContext;
        _logger = logger;
        _userService = userService;
        _usersClient = usersClient;
    }

    public async Task<OneOf<Success, InvalidAuthCode>> Handle(
        AuthUserUsingCodeRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await _userService.GetUserByTelegramUserId(request.TelegramUserId);
        if (existingUser is not null)
        {
            _logger.LogInformation("Telegram user {TelegramUserId} already exists", request.TelegramUserId);
            return default(Success);
        }

        var vbkgUserData = await GetVbkgUser(request.AuthCode);
        if (vbkgUserData is null)
        {
            _logger.LogInformation("Vbkg user by {AuthCode} auth code not found", request.AuthCode);
            return default(InvalidAuthCode);
        }
        var (vbkgUserId, vbkgUserToken) = vbkgUserData.Value;

        _logger.LogDebug("Using {VbkgId} vbk user id to create user", vbkgUserId);

        var user = _userService.CreateUser(
            telegramUserId: request.TelegramUserId,
            vbkgUserId: vbkgUserId,
            vbkgUserToken: vbkgUserToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {TelegramUserId} auth succeeded ({UserId})",
            request.TelegramUserId, user.Id);

        return default(Success);
    }

    private async Task<(long UserId, string UserToken)?> GetVbkgUser(string authCode)
    {
        try
        {
            var response = await _usersClient.UseAuthCodeAsync(new UseAuthCodeRequest
            {
                Source = "vbkg.tgclientbot",
                AuthCode = authCode
            });
            return (response.UserId, response.UserToken);
        }
        catch (RpcException e) when (e.Status.StatusCode == StatusCode.NotFound)
        {
            _logger.LogInformation(e, "Not found rpc exception while requesting to use {AuthCode} auth code", authCode);
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception while requesting to use auth code");
            return null;
        }
    }
}