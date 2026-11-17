using Toons.Domain.Entidades;

namespace Toons.Application.Autenticacion;

public interface IUsuarioRepositorio
{
    Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken cancelacion);

    Task GuardarCambiosAsync(CancellationToken cancelacion);
}
