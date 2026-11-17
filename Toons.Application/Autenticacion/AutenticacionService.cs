using FluentValidation;
using Microsoft.Extensions.Options;
using Toons.Application.Comun;
using Toons.Domain.Entidades;

namespace Toons.Application.Autenticacion;

public sealed class AutenticacionService(
    IValidator<LoginSolicitud> validador,
    IUsuarioRepositorio usuarios,
    IPasswordHasher hasher,
    IGeneradorToken generadorToken,
    IOptions<BloqueoOptions> bloqueo,
    TimeProvider reloj) : IAutenticacionService
{
    public async Task<Resultado<LoginRespuesta>> IniciarSesionAsync(LoginSolicitud solicitud, CancellationToken cancelacion)
    {
        var validacion = await validador.ValidateAsync(solicitud, cancelacion);
        if (!validacion.IsValid)
        {
            var primerError = validacion.Errors[0];
            return Resultado<LoginRespuesta>.Fallo(primerError.ErrorCode, primerError.PropertyName.ToLowerInvariant());
        }

        var correo = solicitud.Correo.Trim().ToLowerInvariant();
        var usuario = await usuarios.ObtenerPorCorreoAsync(correo, cancelacion);
        if (usuario is null)
        {
            hasher.SimularVerificacion(solicitud.Contrasena);
            return Resultado<LoginRespuesta>.Fallo(CodigosError.CredencialesInvalidas);
        }

        var ahora = reloj.GetUtcNow().UtcDateTime;
        if (usuario.EstaBloqueado(ahora))
        {
            return Resultado<LoginRespuesta>.Fallo(CodigosError.CuentaBloqueada);
        }

        if (!hasher.Verificar(solicitud.Contrasena, usuario.ContrasenaHash))
        {
            return await RechazarPorContrasenaIncorrectaAsync(usuario, ahora, cancelacion);
        }

        usuario.ReiniciarIntentos();
        await usuarios.GuardarCambiosAsync(cancelacion);
        return Resultado<LoginRespuesta>.Exito(
            new LoginRespuesta(generadorToken.Generar(usuario), usuario.Nombre, usuario.Rol.Nombre));
    }

    private async Task<Resultado<LoginRespuesta>> RechazarPorContrasenaIncorrectaAsync(
        Usuario usuario, DateTime ahora, CancellationToken cancelacion)
    {
        var opciones = bloqueo.Value;
        usuario.RegistrarIntentoFallido(ahora, opciones.MaximoIntentos, opciones.DuracionBloqueo);
        await usuarios.GuardarCambiosAsync(cancelacion);
        return Resultado<LoginRespuesta>.Fallo(CodigosError.CredencialesInvalidas);
    }
}
