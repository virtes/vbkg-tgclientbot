using Chabot.Command;
using Chabot.Telegram;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using VBkg.TgClientBot.Core.Requests.Background;
using VBkg.TgClientBot.Core.Services.Files;
using VBkg.TgClientBot.Resources;
using VBkg.TgClientBot.Worker.Configuration;
using VBkg.TgClientBot.Worker.States;

namespace VBkg.TgClientBot.Worker.Commands;

public class ProcessPhotoCommandGroup : TelegramCommandGroup
{
    private readonly IMediator _mediator;
    private readonly ITempFileProvider _tempFileProvider;
    private readonly ILogger<ProcessPhotoCommandGroup> _logger;
    private readonly IStringLocalizer<BotLocalization> _localizer;
    private readonly TelegramBotOptions _botOptions;

    public ProcessPhotoCommandGroup(
        IOptions<TelegramBotOptions> botOptions,
        IMediator mediator,
        ITempFileProvider tempFileProvider,
        ILogger<ProcessPhotoCommandGroup> logger,
        IStringLocalizer<BotLocalization> localizer)
    {
        _mediator = mediator;
        _tempFileProvider = tempFileProvider;
        _logger = logger;
        _localizer = localizer;
        _botOptions = botOptions.Value;
    }

    [Command(AllowedWithAnyCommandText = true)]
    [AllowedState(typeof(GroupDefaultState))]
    public async Task ProcessPhotoInGroup()
    {
        if (MessageText?.Contains(_botOptions.Username,
                StringComparison.OrdinalIgnoreCase) is not true)
        {
            return;
        }

        await RemoveBackground(Message);
    }

    [Command(AllowedWithAnyCommandText = true)]
    [AllowedState(typeof(MenuState))]
    public async Task ProcessPhotoInPrivateChat()
    {
        await RemoveBackground(Message);
    }

    private async Task RemoveBackground(Message message)
    {
        var imageMeta = await GetImageMeta(message);
        if (imageMeta is null)
        {
            await BotClient.SendTextMessageAsync(
                chatId: ChatId,
                text: _localizer[nameof(BotLocalization.ImageNotFound)],
                replyToMessageId: MessageId);
            return;
        }

        await using var tempFileStream = _tempFileProvider.CreateTempFile();

        var removeBackgroundRequest = new RemoveBackgroundRequest(
            telegramUserId: UserId,
            imageUrl: imageMeta.Url,
            fileName: imageMeta.Name,
            resultStream: tempFileStream);
        var removeBackgroundResult = await _mediator.Send(removeBackgroundRequest);

        if (!removeBackgroundResult.TryPickT0(out var result, out var error))
        {
            await BotClient.SendTextMessageAsync(ChatId, GetErrorMessage(error),
                replyToMessageId: MessageId);
            return;
        }

        tempFileStream.Seek(0, SeekOrigin.Begin);
        await BotClient.SendMediaGroupAsync(ChatId, new[]
        {
            new InputMediaDocument(new InputMedia(tempFileStream, result.FileName))
        }, replyToMessageId: MessageId);
    }

    private string GetErrorMessage(RemoveBackgroundRequest.Error error)
        => error switch
        {
            RemoveBackgroundRequest.Error.InternalError
                => _localizer[nameof(BotLocalization.RemoveBackground_InternalError)],
            RemoveBackgroundRequest.Error.InsufficientBalance
                => _localizer[nameof(BotLocalization.RemoveBackground_InsufficientBalance), CommandCodes.Deposit],
            RemoveBackgroundRequest.Error.BackgroundNotFound
                => _localizer[nameof(BotLocalization.RemoveBackground_BackgroundNotFound)],
            RemoveBackgroundRequest.Error.NotRegisteredInTelegramClientBot
                => _localizer[nameof(BotLocalization.RemoveBackground_NotRegisteredInTelegramClientBot), _botOptions.Username],
            RemoveBackgroundRequest.Error.FileTooLarge
                => _localizer[nameof(BotLocalization.RemoveBackground_FileTooLarge)],
            RemoveBackgroundRequest.Error.ResolutionTooHigh
                => _localizer[nameof(BotLocalization.RemoveBackground_ImageResolutionTooHigh)],
            RemoveBackgroundRequest.Error.InvalidFileType
                => _localizer[nameof(BotLocalization.RemoveBackground_ImageTypeIsNotSupported)],
            _ => throw new ArgumentOutOfRangeException(nameof(error), error, null)
        };

    private async Task<ImageMeta?> GetImageMeta(Message message)
    {
        async Task<string> GetFileUrl(string fileId)
        {
            var file = await BotClient.GetFileAsync(fileId);
            return $"https://api.telegram.org/file/bot{_botOptions.Token}/{file.FilePath}";
        }

        if (message.Document is not null)
        {
            if (message.Document.MimeType
                is "image/jpeg"
                or "image/png")
            {
                var url = await GetFileUrl(message.Document.FileId);
                var name = message.Document.FileName ??
                           $"image.{(message.Document.MimeType.Contains("jpeg") ? "jpeg": "png")}";
                return new ImageMeta(url, name);
            }
        }

        if (message.Photo is not null)
        {
            var url = await GetFileUrl(message.Photo.Last().FileId);
            return new ImageMeta(url, "image.jpeg");
        }

        _logger.LogDebug("Image not found in {@Message} message", message);

        return null;
    }

    private record ImageMeta(string Url, string Name);
}