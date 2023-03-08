using System.ComponentModel.DataAnnotations;

namespace ZonneConnect.PvOutput;

public class PvOutputSettings
{
    [Required]
    public string ApiKey { get; set; } = String.Empty;

    [Required]
    public string SystemId { get; set; } = String.Empty;
}
