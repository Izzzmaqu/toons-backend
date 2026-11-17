namespace Toons.Domain.Entidades;

public class Favorito
{
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public int ObraId { get; set; }
    public Obra Obra { get; set; } = null!;
    public DateTime FechaMarcado { get; set; }
}
