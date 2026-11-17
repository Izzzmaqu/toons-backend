using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia.Configuraciones;

public class FavoritoConfiguracion : IEntityTypeConfiguration<Favorito>
{
    public void Configure(EntityTypeBuilder<Favorito> favorito)
    {
        favorito.ToTable("favoritos");
        favorito.HasKey(f => new { f.UsuarioId, f.ObraId });
        favorito.HasOne(f => f.Usuario)
            .WithMany(u => u.Favoritos)
            .HasForeignKey(f => f.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
        favorito.HasOne(f => f.Obra)
            .WithMany(o => o.Favoritos)
            .HasForeignKey(f => f.ObraId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
