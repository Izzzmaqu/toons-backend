using FluentValidation;
using Toons.Application.Comun;
using Toons.Domain.Entidades;

namespace Toons.Application.Autenticacion;

public sealed class RegistroService(
    IValidator<RegistroSolicitud> validador,
    IUsuarioRepositorio usuarios,
    IRolRepositorio roles,
    IPasswordHasher hasher,
    IGeneradorToken generadorToken,
    TimeProvider reloj) : IRegistroService
{
    public async Task<Resultado<LoginRespuesta>> RegistrarAsync(RegistroSolicitud solicitud, CancellationToken cancelacion)
    {
        var validacion = await validador.ValidateAsync(solicitud, cancelacion);
        if (!validacion.IsValid)
        {
            return Resultado<LoginRespuesta>.Fallo(validacion.PrimerError());
        }

        var rol = await roles.ObtenerPorNombreAsync(NombresDeRol.Lector, cancelacion)
            ?? throw new InvalidOperationException($"Falta el rol '{NombresDeRol.Lector}' en la base de datos.");
        var usuario = new Usuario
        {
            Nombre = solicitud.Nombre.Trim(),
            Correo = solicitud.Correo.Trim().ToLowerInvariant(),
            ContrasenaHash = hasher.Cifrar(solicitud.Contrasena),
            Rol = rol,
            FechaRegistro = reloj.GetUtcNow().UtcDateTime,
        };

        if (!await usuarios.IntentarAgregarAsync(usuario, cancelacion))
        {
            return Resultado<LoginRespuesta>.Fallo(CodigosError.CorreoEnUso, "correo");
        }

        return Resultado<LoginRespuesta>.Exito(new LoginRespuesta(generadorToken.Generar(usuario), usuario.Nombre, rol.Nombre));
    }
}
