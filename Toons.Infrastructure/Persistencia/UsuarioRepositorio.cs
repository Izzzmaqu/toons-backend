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

    public async Task<bool> IntentarAgregarAsync(Usuario usuario, CancellationToken cancelacion)
    {
        contexto.Usuarios.Add(usuario);
        try
        {
            await contexto.SaveChangesAsync(cancelacion);
            return true;
        }
        catch (DbUpdateException)
        {
            contexto.Entry(usuario).State = EntityState.Detached;
            if (await contexto.Usuarios.AnyAsync(otro => otro.Correo == usuario.Correo, cancelacion))
            {
                return false;
            }

            throw;
        }
    }

    public Task GuardarCambiosAsync(CancellationToken cancelacion)
    {
        return contexto.SaveChangesAsync(cancelacion);
    }
}
