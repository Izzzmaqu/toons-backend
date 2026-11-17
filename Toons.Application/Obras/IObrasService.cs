namespace Toons.Application.Obras;

public interface IObrasService
{
    Task<IReadOnlyList<ObraDto>> ObtenerRecientesAsync(int cantidad, CancellationToken cancelacion);

    Task<IReadOnlyList<ObraDto>> ObtenerTendenciasAsync(int cantidad, CancellationToken cancelacion);

    Task<IReadOnlyList<ObraDto>> BuscarPorTituloAsync(string titulo, CancellationToken cancelacion);
}
