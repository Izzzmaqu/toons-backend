using Toons.Domain.Entidades;

namespace Toons.Application.Autenticacion;

public interface IUsuarioRepositorio
{
    Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken cancelacion);

    Task<bool> IntentarAgregarAsync(Usuario usuario, CancellationToken cancelacion);

    Task GuardarCambiosAsync(CancellationToken cancelacion);
}
