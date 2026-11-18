using Microsoft.EntityFrameworkCore;
using Toons.Application.Autenticacion;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia;

public class RolRepositorio(ToonsDbContext contexto) : IRolRepositorio
{
    public Task<Rol?> ObtenerPorNombreAsync(string nombre, CancellationToken cancelacion)
    {
        return contexto.Roles.FirstOrDefaultAsync(rol => rol.Nombre == nombre, cancelacion);
    }
}
