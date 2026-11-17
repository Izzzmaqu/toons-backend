using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia.Configuraciones;

public class ObraEtiquetaConfiguracion : IEntityTypeConfiguration<ObraEtiqueta>
{
    public void Configure(EntityTypeBuilder<ObraEtiqueta> obraEtiqueta)
    {
        obraEtiqueta.ToTable("obras_etiquetas");
        obraEtiqueta.HasKey(oe => new { oe.ObraId, oe.EtiquetaId });
        obraEtiqueta.HasOne(oe => oe.Obra)
            .WithMany(o => o.ObrasEtiquetas)
            .HasForeignKey(oe => oe.ObraId)
            .OnDelete(DeleteBehavior.Cascade);
        obraEtiqueta.HasOne(oe => oe.Etiqueta)
            .WithMany(e => e.ObrasEtiquetas)
            .HasForeignKey(oe => oe.EtiquetaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
