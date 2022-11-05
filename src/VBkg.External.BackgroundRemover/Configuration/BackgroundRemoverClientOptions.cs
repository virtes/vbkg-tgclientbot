using System.ComponentModel.DataAnnotations;

namespace VBkg.External.BackgroundRemover.Configuration;

public class BackgroundRemoverClientOptions
{
    [Required(AllowEmptyStrings = false)]
    public string Host { get; set; } = default!;
}