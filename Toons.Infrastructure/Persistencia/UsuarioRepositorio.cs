using Microsoft.EntityFrameworkCore;
using Toons.Application.Autenticacion;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia;

public class UsuarioRepositorio(ToonsDbContext contexto) : IUsuarioRepositorio
{
    public Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken cancelacion)
    {
        return contexto.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Correo == correo, cancelacion);
    }

    public Task GuardarCambiosAsync(CancellationToken cancelacion)
    {
        return contexto.SaveChangesAsync(cancelacion);
    }
}
