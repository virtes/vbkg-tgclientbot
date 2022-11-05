using OneOf;

namespace VBkg.External.BackgroundRemover.Dto;

[GenerateOneOf]
public partial class RemoveBackgroundResponseDto
    : OneOfBase<RemoveBackgroundErrorResponseDto, RemoveBackgroundSuccessResponseDto>
{
}