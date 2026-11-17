namespace Toons.Domain.Entidades;

public class Etiqueta
{
    public int Id { get; set; }
    public required string Nombre { get; set; }
    public ICollection<ObraEtiqueta> ObrasEtiquetas { get; set; } = [];
}
