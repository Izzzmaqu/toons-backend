using Toons.Domain.Entidades;

namespace Toons.Application.Autenticacion;

public interface IRolRepositorio
{
    Task<Rol?> ObtenerPorNombreAsync(string nombre, CancellationToken cancelacion);
}
