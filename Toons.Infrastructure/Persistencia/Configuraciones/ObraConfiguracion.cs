using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia.Configuraciones;

public class ObraConfiguracion : IEntityTypeConfiguration<Obra>
{
    public void Configure(EntityTypeBuilder<Obra> obra)
    {
        obra.ToTable("obras");
        obra.HasKey(o => o.Id);
        obra.Property(o => o.Titulo).HasMaxLength(200).IsRequired();
        obra.Property(o => o.Descripcion).HasMaxLength(1000).IsRequired();
        obra.Property(o => o.Portada).HasMaxLength(100).IsRequired();
        obra.HasOne(o => o.Creador)
            .WithMany(u => u.ObrasCreadas)
            .HasForeignKey(o => o.CreadorId)
            .OnDelete(DeleteBehavior.Restrict);
        obra.HasOne(o => o.Idioma)
            .WithMany(i => i.Obras)
            .HasForeignKey(o => o.IdiomaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
