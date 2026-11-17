namespace Toons.Domain.Entidades;

public class Lectura
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public int CapituloId { get; set; }
    public Capitulo Capitulo { get; set; } = null!;
    public DateTime FechaLectura { get; set; }
}
