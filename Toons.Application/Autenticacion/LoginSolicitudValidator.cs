using FluentValidation;
using Toons.Application.Comun;

namespace Toons.Application.Autenticacion;

public sealed class LoginSolicitudValidator : AbstractValidator<LoginSolicitud>
{
    private const int LargoMinimoContrasena = 8;

    public LoginSolicitudValidator()
    {
        RuleFor(solicitud => solicitud.Correo)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithErrorCode(CodigosError.CorreoObligatorio)
            .EmailAddress().WithErrorCode(CodigosError.CorreoInvalido);

        RuleFor(solicitud => solicitud.Contrasena)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithErrorCode(CodigosError.ContrasenaObligatoria)
            .MinimumLength(LargoMinimoContrasena).WithErrorCode(CodigosError.ContrasenaCorta);
    }
}
