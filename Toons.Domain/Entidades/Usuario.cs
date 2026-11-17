namespace Toons.Domain.Entidades;

public class Usuario
{
    public int Id { get; set; }
    public required string Nombre { get; set; }
    public required string Correo { get; set; }
    public required string ContrasenaHash { get; set; }
    public int RolId { get; set; }
    public Rol Rol { get; set; } = null!;
    public int IntentosFallidos { get; private set; }
    public DateTimeOffset? BloqueadoHasta { get; private set; }
    public DateTimeOffset FechaRegistro { get; set; }

    public bool EstaBloqueado(DateTimeOffset ahora)
    {
        return BloqueadoHasta > ahora;
    }

    public void RegistrarIntentoFallido(DateTimeOffset ahora, int maximoIntentos, TimeSpan duracionBloqueo)
    {
        if (BloqueadoHasta <= ahora)
        {
            ReiniciarIntentos();
        }

        IntentosFallidos++;

        if (IntentosFallidos >= maximoIntentos)
        {
            BloqueadoHasta = ahora + duracionBloqueo;
        }
    }

    public void ReiniciarIntentos()
    {
        IntentosFallidos = 0;
        BloqueadoHasta = null;
    }
}
