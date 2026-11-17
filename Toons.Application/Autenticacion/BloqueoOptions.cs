using System.ComponentModel.DataAnnotations;

namespace Toons.Application.Autenticacion;

public sealed class BloqueoOptions
{
    public const string Seccion = "Bloqueo";

    [Range(1, 100)]
    public int MaximoIntentos { get; set; } = 5;

    [Range(1, 1440)]
    public int MinutosBloqueo { get; set; } = 5;

    public TimeSpan DuracionBloqueo => TimeSpan.FromMinutes(MinutosBloqueo);
}
