using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia.Configuraciones;

public class PaginaConfiguracion : IEntityTypeConfiguration<Pagina>
{
    public void Configure(EntityTypeBuilder<Pagina> pagina)
    {
        pagina.ToTable("paginas");
        pagina.HasKey(p => p.Id);
        pagina.Property(p => p.UrlImagen).HasMaxLength(500).IsRequired();
        pagina.HasIndex(p => new { p.CapituloId, p.Numero }).IsUnique();
        pagina.HasOne(p => p.Capitulo)
            .WithMany(c => c.Paginas)
            .HasForeignKey(p => p.CapituloId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
