using Microsoft.AspNetCore.Mvc;
using Toons.Application.Comun;

namespace Toons.Api.Errores;

public sealed record RespuestaError(string Codigo, string? Campo)
{
    public static ObjectResult Desde(ErrorAplicacion error)
    {
        return new ObjectResult(new RespuestaError(error.Codigo, error.Campo))
        {
            StatusCode = EstadoHttp(error.Codigo),
        };
    }

    private static int EstadoHttp(string codigo) => codigo switch
    {
        CodigosError.CredencialesInvalidas => StatusCodes.Status401Unauthorized,
        CodigosError.CuentaBloqueada => StatusCodes.Status423Locked,
        CodigosError.CorreoEnUso => StatusCodes.Status409Conflict,
        CodigosError.ErrorInterno => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status400BadRequest,
    };
}
