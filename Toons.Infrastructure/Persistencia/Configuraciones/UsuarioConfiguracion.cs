using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia.Configuraciones;

public class UsuarioConfiguracion : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> usuario)
    {
        usuario.ToTable("usuarios");
        usuario.HasKey(u => u.Id);
        usuario.Property(u => u.Nombre).HasMaxLength(100).IsRequired();
        usuario.Property(u => u.Correo).HasMaxLength(254).IsRequired();
        usuario.Property(u => u.ContrasenaHash).HasMaxLength(500).IsRequired();
        usuario.HasIndex(u => u.Correo).IsUnique();
        usuario.HasOne(u => u.Rol)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.RolId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
