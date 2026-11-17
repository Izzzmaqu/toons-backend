using Microsoft.EntityFrameworkCore;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia;

public class ToonsDbContext(DbContextOptions<ToonsDbContext> opciones) : DbContext(opciones)
{
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ToonsDbContext).Assembly);
    }
}
