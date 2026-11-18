using Microsoft.AspNetCore.Http;
using Toons.Api.Errores;
using Toons.Application.Comun;

namespace Toons.Tests.Api;

public class RespuestaErrorPruebas
{
    [Theory]
    [InlineData(CodigosError.CredencialesInvalidas, StatusCodes.Status401Unauthorized)]
    [InlineData(CodigosError.CuentaBloqueada, StatusCodes.Status423Locked)]
    [InlineData(CodigosError.CorreoObligatorio, StatusCodes.Status400BadRequest)]
    [InlineData(CodigosError.CorreoInvalido, StatusCodes.Status400BadRequest)]
    [InlineData(CodigosError.ContrasenaObligatoria, StatusCodes.Status400BadRequest)]
    [InlineData(CodigosError.ContrasenaCorta, StatusCodes.Status400BadRequest)]
    [InlineData(CodigosError.NombreObligatorio, StatusCodes.Status400BadRequest)]
    [InlineData(CodigosError.NombreLargo, StatusCodes.Status400BadRequest)]
    [InlineData(CodigosError.CorreoEnUso, StatusCodes.Status409Conflict)]
    [InlineData(CodigosError.SolicitudInvalida, StatusCodes.Status400BadRequest)]
    [InlineData(CodigosError.ErrorInterno, StatusCodes.Status500InternalServerError)]
    public void Cada_codigo_se_traduce_a_su_estado_http(string codigo, int estadoEsperado)
    {
        var resultado = RespuestaError.Desde(new ErrorAplicacion(codigo));

        Assert.Equal(estadoEsperado, resultado.StatusCode);
    }

    [Fact]
    public void El_cuerpo_lleva_solo_codigo_y_campo()
    {
        var resultado = RespuestaError.Desde(new ErrorAplicacion(CodigosError.CorreoInvalido, "correo"));

        var cuerpo = Assert.IsType<RespuestaError>(resultado.Value);
        Assert.Equal(new RespuestaError(CodigosError.CorreoInvalido, "correo"), cuerpo);
    }
}
