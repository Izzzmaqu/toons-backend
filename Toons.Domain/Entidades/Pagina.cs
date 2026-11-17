namespace Toons.Domain.Entidades;

public class Pagina
{
    public int Id { get; set; }
    public int CapituloId { get; set; }
    public Capitulo Capitulo { get; set; } = null!;
    public int Numero { get; set; }
    public required string UrlImagen { get; set; }
}
