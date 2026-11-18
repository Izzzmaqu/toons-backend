using FluentValidation;
using Toons.Application.Comun;

namespace Toons.Application.Autenticacion;

public sealed class RegistroSolicitudValidator : AbstractValidator<RegistroSolicitud>
{
    private const int LargoMaximoNombre = 60;

    public RegistroSolicitudValidator()
    {
        RuleFor(solicitud => solicitud.Nombre)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithErrorCode(CodigosError.NombreObligatorio)
            .MaximumLength(LargoMaximoNombre).WithErrorCode(CodigosError.NombreLargo);
        RuleFor(solicitud => solicitud.Correo).ConCorreoValido();
        RuleFor(solicitud => solicitud.Contrasena).ConContrasenaValida();
    }
}
