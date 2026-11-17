namespace Toons.Domain.Entidades;

public class Comentario
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public int ObraId { get; set; }
    public Obra Obra { get; set; } = null!;
    public required string Texto { get; set; }
    public DateTime Fecha { get; set; }
}
