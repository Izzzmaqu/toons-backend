namespace Toons.Domain.Entidades;

public class Capitulo
{
    public int Id { get; set; }
    public int ObraId { get; set; }
    public Obra Obra { get; set; } = null!;
    public int Numero { get; set; }
    public required string Titulo { get; set; }
    public DateTime FechaPublicacion { get; set; }
    public ICollection<Pagina> Paginas { get; set; } = [];
    public ICollection<Lectura> Lecturas { get; set; } = [];
}
