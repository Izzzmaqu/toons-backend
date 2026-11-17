using Microsoft.EntityFrameworkCore;
using Toons.Application.Autenticacion;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia;

public class SembradorDatos(ToonsDbContext contexto, IPasswordHasher hasher, TimeProvider reloj)
{
    public async Task SembrarAsync(CancellationToken cancelacion)
    {
        if (await contexto.Roles.AnyAsync(cancelacion))
        {
            return;
        }

        var rolAdministrador = new Rol { Nombre = "administrador" };
        var rolLector = new Rol { Nombre = "lector" };
        contexto.Roles.AddRange(rolAdministrador, rolLector);
        contexto.Usuarios.AddRange(
            CrearUsuario("Admin de ejemplo", "admin@ejemplo.com", "admin-ejemplo-123", rolAdministrador),
            CrearUsuario("Usuario de ejemplo", "usuario@ejemplo.com", "usuario-ejemplo-123", rolLector));
        await contexto.SaveChangesAsync(cancelacion);
    }

    private Usuario CrearUsuario(string nombre, string correo, string contrasena, Rol rol)
    {
        return new Usuario
        {
            Nombre = nombre,
            Correo = correo,
            ContrasenaHash = hasher.Cifrar(contrasena),
            Rol = rol,
            FechaRegistro = reloj.GetUtcNow(),
        };
    }
}
