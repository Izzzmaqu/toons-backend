using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia.Configuraciones;

public class IdiomaConfiguracion : IEntityTypeConfiguration<Idioma>
{
    public void Configure(EntityTypeBuilder<Idioma> idioma)
    {
        idioma.ToTable("idiomas");
        idioma.HasKey(i => i.Id);
        idioma.Property(i => i.Nombre).HasMaxLength(50).IsRequired();
        idioma.HasIndex(i => i.Nombre).IsUnique();
    }
}
