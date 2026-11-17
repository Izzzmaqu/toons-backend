using System.ComponentModel.DataAnnotations;

namespace Toons.Infrastructure.Seguridad;

public sealed class JwtOptions
{
    public const string Seccion = "Jwt";

    [Required, MinLength(32)]
    public string Clave { get; set; } = string.Empty;

    [Required]
    public string Emisor { get; set; } = string.Empty;

    [Required]
    public string Audiencia { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int MinutosVigencia { get; set; } = 60;
}
