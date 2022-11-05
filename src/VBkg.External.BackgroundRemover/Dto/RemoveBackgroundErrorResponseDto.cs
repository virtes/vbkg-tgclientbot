using System.Text.Json.Serialization;

namespace VBkg.External.BackgroundRemover.Dto;

public class RemoveBackgroundErrorResponseDto
{
    [JsonPropertyName("Error")]
    public RemoveBackgroundErrorDto Error { get; set; }
}