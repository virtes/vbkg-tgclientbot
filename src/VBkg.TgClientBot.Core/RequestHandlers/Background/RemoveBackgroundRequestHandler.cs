using MediatR;
using OneOf;
using Microsoft.Extensions.Logging;
using VBkg.External.BackgroundRemover;
using VBkg.External.BackgroundRemover.Dto;
using VBkg.TgClientBot.Core.Requests.Background;
using VBkg.TgClientBot.Core.Services.Users;
using SuccessResult = VBkg.TgClientBot.Core.Requests.Background.RemoveBackgroundRequest.SuccessResult;
using Error = VBkg.TgClientBot.Core.Requests.Background.RemoveBackgroundRequest.Error;

namespace VBkg.TgClientBot.Core.RequestHandlers.Background;

public class RemoveBackgroundRequestHandler
    : IRequestHandler<RemoveBackgroundRequest, OneOf<SuccessResult, Error>>
{
    private readonly IBackgroundRemoverClient _backgroundRemoverClient;
    private readonly IUserService _userService;
    private readonly ILogger<RemoveBackgroundRequestHandler> _logger;

    public RemoveBackgroundRequestHandler(
        IBackgroundRemoverClient backgroundRemoverClient,
        IUserService userService,
        ILogger<RemoveBackgroundRequestHandler> logger)
    {
        _backgroundRemoverClient = backgroundRemoverClient;
        _userService = userService;
        _logger = logger;
    }

    public async Task<OneOf<SuccessResult, Error>>
        Handle(RemoveBackgroundRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByTelegramUserId(request.TelegramUserId);
        if (user is null)
        {
            _logger.LogInformation("User by {TelegramUserId} telegram ud not found",
                request.TelegramUserId);
            return Error.NotRegisteredInTelegramClientBot;
        }

        RemoveBackgroundResponseDto removeBackgroundResponse;
        try
        {
            removeBackgroundResponse = await _backgroundRemoverClient
                .RemoveBackground(new RemoveBackgroundRequestDto
                {
                    ImageUrl = request.ImageUrl,
                    UserToken = user.VbkgUserToken,
                    FileName = request.FileName
                }, request.ResultStream);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception while removing background for {UserId} user",
                user.Id);
            return Error.InternalError;
        }

        if (removeBackgroundResponse.TryPickT0(out var error, out var success))
        {
            _logger.LogInformation("Background removing failed for {UserId} ({@Error})",
                user.Id, error);

            return error.Error switch
            {
                RemoveBackgroundErrorDto.insufficient_credits => Error.InsufficientBalance,
                RemoveBackgroundErrorDto.unknown_foreground => Error.BackgroundNotFound,
                RemoveBackgroundErrorDto.file_too_large => Error.FileTooLarge,
                RemoveBackgroundErrorDto.image_resolution_too_high => Error.ResolutionTooHigh,
                RemoveBackgroundErrorDto.invalid_file_type => Error.InvalidFileType,
                RemoveBackgroundErrorDto.user_not_found => Error.UserNotFound,
                RemoveBackgroundErrorDto.internal_error => Error.InternalError,
                _ => throw new ArgumentOutOfRangeException(nameof(error.Error), error.Error, null)
            };
        }

        if (request.ResultStream.Length == 0)
        {
            _logger.LogError("Remove background response is success, but result stream is empty");
            return Error.InternalError;
        }

        _logger.LogInformation("Background removed successfully " +
                               "for {UserId} user ({ResultStreamLength})",
            user.Id, request.ResultStream.Length);

        return new SuccessResult(success.FileName);
    }
}