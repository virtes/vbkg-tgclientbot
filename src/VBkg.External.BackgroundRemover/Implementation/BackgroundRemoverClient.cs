using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using VBkg.External.BackgroundRemover.Dto;

namespace VBkg.External.BackgroundRemover.Implementation;

public class BackgroundRemoverClient : IBackgroundRemoverClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BackgroundRemoverClient> _logger;

    public BackgroundRemoverClient(HttpClient httpClient,
        ILogger<BackgroundRemoverClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<RemoveBackgroundResponseDto> RemoveBackground(
        RemoveBackgroundRequestDto request, Stream resultStream)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post,
            "api/photos/removeBackground")
        {
            Content = JsonContent.Create(request)
        };
        using var responseMessage = await _httpClient.SendAsync(requestMessage,
            HttpCompletionOption.ResponseHeadersRead);

        if (responseMessage.StatusCode == HttpStatusCode.OK)
        {
            await responseMessage.Content.CopyToAsync(resultStream);

            return new RemoveBackgroundSuccessResponseDto
            {
                FileName = responseMessage.Headers
                    .GetValues("ResultFileName")
                    .First()
            };
        }

        return await DeserializeResponse<RemoveBackgroundErrorResponseDto>(responseMessage);
    }

    private async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var stringResponse = await response.Content.ReadAsStringAsync();

        try
        {
            return JsonSerializer.Deserialize<T>(stringResponse)!;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception while deserializing {StringResponse} to {TargetType} type",
                stringResponse, typeof(T).FullName);
            throw;
        }
    }
}