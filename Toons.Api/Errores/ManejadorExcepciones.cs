using Microsoft.AspNetCore.Diagnostics;
using Toons.Application.Comun;

namespace Toons.Api.Errores;

public sealed class ManejadorExcepciones(ILogger<ManejadorExcepciones> registro) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext contexto, Exception excepcion, CancellationToken cancelacion)
    {
        registro.LogError(excepcion, "Error no controlado");
        contexto.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await contexto.Response.WriteAsJsonAsync(
            new RespuestaError(CodigosError.ErrorInterno, null), cancelacion);
        return true;
    }
}
