using System.Text.Json.Serialization;

namespace VBkg.External.BackgroundRemover.Dto;

public class RemoveBackgroundRequestDto
{
    [JsonPropertyName("UserToken")]
    public string UserToken { get; set; } = default!;

    [JsonPropertyName("ImageUrl")]
    public string ImageUrl { get; set; } = default!;

    [JsonPropertyName("FileName")]
    public string FileName { get; set; } = default!;
}