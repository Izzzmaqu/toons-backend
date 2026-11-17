using Microsoft.EntityFrameworkCore;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia;

public class ToonsDbContext(DbContextOptions<ToonsDbContext> opciones) : DbContext(opciones)
{
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Idioma> Idiomas => Set<Idioma>();
    public DbSet<Obra> Obras => Set<Obra>();
    public DbSet<Etiqueta> Etiquetas => Set<Etiqueta>();
    public DbSet<ObraEtiqueta> ObrasEtiquetas => Set<ObraEtiqueta>();
    public DbSet<Capitulo> Capitulos => Set<Capitulo>();
    public DbSet<Pagina> Paginas => Set<Pagina>();
    public DbSet<Favorito> Favoritos => Set<Favorito>();
    public DbSet<Lectura> Lecturas => Set<Lectura>();
    public DbSet<Comentario> Comentarios => Set<Comentario>();
    public DbSet<Calificacion> Calificaciones => Set<Calificacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ToonsDbContext).Assembly);
    }
}
