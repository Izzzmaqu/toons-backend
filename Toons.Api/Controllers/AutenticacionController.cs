using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toons.Api.Errores;
using Toons.Application.Autenticacion;
using Toons.Infrastructure.Seguridad;

namespace Toons.Api.Controllers;

[ApiController]
[Route("api/autenticacion")]
public class AutenticacionController(IAutenticacionService autenticacion) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> IniciarSesion(LoginSolicitud solicitud, CancellationToken cancelacion)
    {
        var resultado = await autenticacion.IniciarSesionAsync(solicitud, cancelacion);
        return resultado.EsExitoso ? Ok(resultado.Valor) : RespuestaError.Desde(resultado.Error);
    }

    [Authorize]
    [HttpGet("perfil")]
    public IActionResult ObtenerPerfil()
    {
        var nombre = User.FindFirstValue(GeneradorTokenJwt.TipoClaimNombre);
        var rol = User.FindFirstValue(GeneradorTokenJwt.TipoClaimRol);
        return Ok(new { nombre, rol });
    }
}
