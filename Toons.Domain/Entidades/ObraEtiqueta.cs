namespace Toons.Domain.Entidades;

public class ObraEtiqueta
{
    public int ObraId { get; set; }
    public Obra Obra { get; set; } = null!;
    public int EtiquetaId { get; set; }
    public Etiqueta Etiqueta { get; set; } = null!;
}
