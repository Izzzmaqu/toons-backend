using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia.Configuraciones;

public class CalificacionConfiguracion : IEntityTypeConfiguration<Calificacion>
{
    public void Configure(EntityTypeBuilder<Calificacion> calificacion)
    {
        calificacion.ToTable(
            "calificaciones",
            tabla => tabla.HasCheckConstraint("ck_calificaciones_puntaje", "puntaje BETWEEN 1 AND 5"));
        calificacion.HasKey(c => c.Id);
        calificacion.HasIndex(c => new { c.UsuarioId, c.ObraId }).IsUnique();
        calificacion.HasOne(c => c.Usuario)
            .WithMany(u => u.Calificaciones)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
        calificacion.HasOne(c => c.Obra)
            .WithMany(o => o.Calificaciones)
            .HasForeignKey(c => c.ObraId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
