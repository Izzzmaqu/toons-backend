using FluentValidation.Results;

namespace Toons.Application.Comun;

internal static class ResultadoDeValidacionExtensiones
{
    public static ErrorAplicacion PrimerError(this ValidationResult validacion)
    {
        var error = validacion.Errors[0];
        return new ErrorAplicacion(error.ErrorCode, error.PropertyName.ToLowerInvariant());
    }
}
