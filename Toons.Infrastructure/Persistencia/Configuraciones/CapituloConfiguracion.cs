using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia.Configuraciones;

public class CapituloConfiguracion : IEntityTypeConfiguration<Capitulo>
{
    public void Configure(EntityTypeBuilder<Capitulo> capitulo)
    {
        capitulo.ToTable("capitulos");
        capitulo.HasKey(c => c.Id);
        capitulo.Property(c => c.Titulo).HasMaxLength(200).IsRequired();
        capitulo.HasIndex(c => new { c.ObraId, c.Numero }).IsUnique();
        capitulo.HasOne(c => c.Obra)
            .WithMany(o => o.Capitulos)
            .HasForeignKey(c => c.ObraId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
