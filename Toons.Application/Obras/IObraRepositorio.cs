namespace Toons.Application.Obras;

public interface IObraRepositorio
{
    Task<IReadOnlyList<ObraDto>> ObtenerRecientesAsync(int cantidad, CancellationToken cancelacion);

    Task<IReadOnlyList<ObraDto>> ObtenerTendenciasAsync(DateTime desde, int cantidad, CancellationToken cancelacion);

    Task<IReadOnlyList<ObraDto>> BuscarPorTituloAsync(string titulo, CancellationToken cancelacion);
}
