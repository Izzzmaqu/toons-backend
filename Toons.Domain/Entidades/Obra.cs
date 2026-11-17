namespace Toons.Domain.Entidades;

public class Obra
{
    public int Id { get; set; }
    public required string Titulo { get; set; }
    public required string Descripcion { get; set; }
    public required string Portada { get; set; }
    public int CreadorId { get; set; }
    public Usuario Creador { get; set; } = null!;
    public int IdiomaId { get; set; }
    public Idioma Idioma { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
    public ICollection<ObraEtiqueta> ObrasEtiquetas { get; set; } = [];
    public ICollection<Capitulo> Capitulos { get; set; } = [];
    public ICollection<Favorito> Favoritos { get; set; } = [];
    public ICollection<Comentario> Comentarios { get; set; } = [];
    public ICollection<Calificacion> Calificaciones { get; set; } = [];
}
