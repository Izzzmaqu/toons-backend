namespace Toons.Domain.Entidades;

public class Idioma
{
    public int Id { get; set; }
    public required string Nombre { get; set; }
    public ICollection<Obra> Obras { get; set; } = [];
}
