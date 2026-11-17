using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toons.Application.Obras;

namespace Toons.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/obras")]
public class ObrasController(IObrasService obras) : ControllerBase
{
    private const int CantidadPorFila = 10;

    [HttpGet("recientes")]
    public async Task<IReadOnlyList<ObraDto>> ObtenerRecientes(CancellationToken cancelacion)
    {
        return await obras.ObtenerRecientesAsync(CantidadPorFila, cancelacion);
    }

    [HttpGet("tendencias")]
    public async Task<IReadOnlyList<ObraDto>> ObtenerTendencias(CancellationToken cancelacion)
    {
        return await obras.ObtenerTendenciasAsync(CantidadPorFila, cancelacion);
    }

    [HttpGet]
    public async Task<IReadOnlyList<ObraDto>> BuscarPorTitulo([FromQuery] string? titulo, CancellationToken cancelacion)
    {
        return await obras.BuscarPorTituloAsync(titulo ?? string.Empty, cancelacion);
    }
}
