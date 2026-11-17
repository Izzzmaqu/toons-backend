namespace Toons.Application.Obras;

public sealed class ObrasService(IObraRepositorio obras, TimeProvider reloj) : IObrasService
{
    private const int DiasDeTendencia = 30;

    public Task<IReadOnlyList<ObraDto>> ObtenerRecientesAsync(int cantidad, CancellationToken cancelacion)
    {
        return obras.ObtenerRecientesAsync(cantidad, cancelacion);
    }

    public Task<IReadOnlyList<ObraDto>> ObtenerTendenciasAsync(int cantidad, CancellationToken cancelacion)
    {
        var desde = reloj.GetUtcNow().UtcDateTime.AddDays(-DiasDeTendencia);
        return obras.ObtenerTendenciasAsync(desde, cantidad, cancelacion);
    }

    public Task<IReadOnlyList<ObraDto>> BuscarPorTituloAsync(string titulo, CancellationToken cancelacion)
    {
        return obras.BuscarPorTituloAsync(titulo.Trim(), cancelacion);
    }
}
