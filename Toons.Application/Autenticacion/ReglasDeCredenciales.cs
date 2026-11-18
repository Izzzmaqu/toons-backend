using FluentValidation;
using Toons.Application.Comun;

namespace Toons.Application.Autenticacion;

internal static class ReglasDeCredenciales
{
    private const int LargoMinimoContrasena = 8;

    public static IRuleBuilderOptions<T, string> ConCorreoValido<T>(this IRuleBuilderInitial<T, string> regla)
    {
        return regla
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithErrorCode(CodigosError.CorreoObligatorio)
            .EmailAddress().WithErrorCode(CodigosError.CorreoInvalido);
    }

    public static IRuleBuilderOptions<T, string> ConContrasenaValida<T>(this IRuleBuilderInitial<T, string> regla)
    {
        return regla
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithErrorCode(CodigosError.ContrasenaObligatoria)
            .MinimumLength(LargoMinimoContrasena).WithErrorCode(CodigosError.ContrasenaCorta);
    }
}
