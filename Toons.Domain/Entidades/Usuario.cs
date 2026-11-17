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
    public DateTime? BloqueadoHasta { get; private set; }
    public DateTime FechaRegistro { get; set; }
    public ICollection<Obra> ObrasCreadas { get; set; } = [];
    public ICollection<Favorito> Favoritos { get; set; } = [];
    public ICollection<Lectura> Lecturas { get; set; } = [];
    public ICollection<Comentario> Comentarios { get; set; } = [];
    public ICollection<Calificacion> Calificaciones { get; set; } = [];

    public bool EstaBloqueado(DateTime ahora)
    {
        return BloqueadoHasta > ahora;
    }

    public void RegistrarIntentoFallido(DateTime ahora, int maximoIntentos, TimeSpan duracionBloqueo)
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
