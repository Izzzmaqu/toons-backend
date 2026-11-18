using FluentValidation;

namespace Toons.Application.Autenticacion;

public sealed class LoginSolicitudValidator : AbstractValidator<LoginSolicitud>
{
    public LoginSolicitudValidator()
    {
        RuleFor(solicitud => solicitud.Correo).ConCorreoValido();
        RuleFor(solicitud => solicitud.Contrasena).ConContrasenaValida();
    }
}
