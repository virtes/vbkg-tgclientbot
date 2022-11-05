namespace VBkg.External.BackgroundRemover.Dto;

// ReSharper disable InconsistentNaming
public enum RemoveBackgroundErrorDto
{
    insufficient_credits,
    unknown_foreground,
    file_too_large,
    image_resolution_too_high,
    invalid_file_type,
    user_not_found,
    internal_error
}
// ReSharper restore InconsistentNaming