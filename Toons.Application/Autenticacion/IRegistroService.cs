using Toons.Application.Comun;

namespace Toons.Application.Autenticacion;

public interface IRegistroService
{
    Task<Resultado<LoginRespuesta>> RegistrarAsync(RegistroSolicitud solicitud, CancellationToken cancelacion);
}
