using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia.Configuraciones;

public class ComentarioConfiguracion : IEntityTypeConfiguration<Comentario>
{
    public void Configure(EntityTypeBuilder<Comentario> comentario)
    {
        comentario.ToTable("comentarios");
        comentario.HasKey(c => c.Id);
        comentario.Property(c => c.Texto).HasMaxLength(1000).IsRequired();
        comentario.HasOne(c => c.Usuario)
            .WithMany(u => u.Comentarios)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
        comentario.HasOne(c => c.Obra)
            .WithMany(o => o.Comentarios)
            .HasForeignKey(c => c.ObraId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
