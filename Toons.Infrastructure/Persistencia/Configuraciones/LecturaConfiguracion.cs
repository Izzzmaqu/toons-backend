using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia.Configuraciones;

public class LecturaConfiguracion : IEntityTypeConfiguration<Lectura>
{
    public void Configure(EntityTypeBuilder<Lectura> lectura)
    {
        lectura.ToTable("lecturas");
        lectura.HasKey(l => l.Id);
        lectura.HasIndex(l => l.FechaLectura);
        lectura.HasOne(l => l.Usuario)
            .WithMany(u => u.Lecturas)
            .HasForeignKey(l => l.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
        lectura.HasOne(l => l.Capitulo)
            .WithMany(c => c.Lecturas)
            .HasForeignKey(l => l.CapituloId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
