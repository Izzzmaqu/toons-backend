using Toons.Application.Comun;

namespace Toons.Application.Autenticacion;

public interface IAutenticacionService
{
    Task<Resultado<LoginRespuesta>> IniciarSesionAsync(LoginSolicitud solicitud, CancellationToken cancelacion);
}
