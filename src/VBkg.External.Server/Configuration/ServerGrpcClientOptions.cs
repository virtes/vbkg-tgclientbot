using System.ComponentModel.DataAnnotations;

namespace VBkg.External.Server.Configuration;

public class ServerGrpcClientOptions
{
    [Required(AllowEmptyStrings = false)]
    public string Host { get; set; } = default!;

    [Required(AllowEmptyStrings = false)]
    public string ApiKey { get; set; } = default!;
}