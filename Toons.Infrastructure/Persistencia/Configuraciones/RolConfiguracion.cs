using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia.Configuraciones;

public class RolConfiguracion : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> rol)
    {
        rol.ToTable("roles");
        rol.HasKey(r => r.Id);
        rol.Property(r => r.Nombre).HasMaxLength(50).IsRequired();
        rol.HasIndex(r => r.Nombre).IsUnique();
    }
}
