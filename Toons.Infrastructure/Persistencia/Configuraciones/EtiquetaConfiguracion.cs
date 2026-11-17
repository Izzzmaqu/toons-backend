using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia.Configuraciones;

public class EtiquetaConfiguracion : IEntityTypeConfiguration<Etiqueta>
{
    public void Configure(EntityTypeBuilder<Etiqueta> etiqueta)
    {
        etiqueta.ToTable("etiquetas");
        etiqueta.HasKey(e => e.Id);
        etiqueta.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
        etiqueta.HasIndex(e => e.Nombre).IsUnique();
    }
}
