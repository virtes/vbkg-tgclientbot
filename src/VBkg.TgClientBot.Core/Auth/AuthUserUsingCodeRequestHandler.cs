using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using VBkg.TgClientBot.Contracts.Auth;
using VBkg.TgClientBot.Data;
using VBkg.TgClientBot.Proto.Server;
using VBkg.TgClientBot.Services.Users;

namespace VBkg.TgClientBot.Core.Auth;

public class AuthUserUsingCodeRequestHandler
    : IRequestHandler<AuthUserUsingCodeRequest, Either<SuccessAuth, InvalidAuthCode>>
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

    public async Task<Either<SuccessAuth, InvalidAuthCode>> Handle(
        AuthUserUsingCodeRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await _userService.GetUserByTelegramUserId(request.TelegramUserId);
        if (existingUser is not null)
        {
            _logger.LogInformation("Telegram user {TelegramUserId} already exists", request.TelegramUserId);
            return default(SuccessAuth);
        }

        var vbkgId = await GetVbkgId(request.AuthCode);
        if (vbkgId is null)
        {
            _logger.LogInformation("Vbkg user by {AuthCode} auth code not found", request.AuthCode);
            return default(InvalidAuthCode);
        }

        _logger.LogDebug("Using {VbkgId} vbk user id to create user", vbkgId.Value);

        var user = _userService.CreateUser(
            telegramUserId: request.TelegramUserId,
            vbkgId: vbkgId.Value);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {TelegramUserId} auth succeeded ({UserId})",
            request.TelegramUserId, user.Id);

        return default(SuccessAuth);
    }

    private async Task<long?> GetVbkgId(string authCode)
    {
        try
        {
            var response = await _usersClient.UseAuthCodeAsync(new UseAuthCodeRequest
            {
                Source = "vbkg.tgclientbot",
                AuthCode = authCode
            });
            return response.UserId;
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