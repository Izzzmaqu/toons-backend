using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Toons.Application.Obras;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia;

public class ObraRepositorio(ToonsDbContext contexto) : IObraRepositorio
{
    private static readonly Expression<Func<Obra, ObraDto>> AObraDto = obra => new ObraDto(
        obra.Id,
        obra.Titulo,
        obra.Descripcion,
        obra.Portada,
        obra.Creador.Nombre,
        obra.Capitulos.Max(capitulo => (DateTime?)capitulo.FechaPublicacion) ?? obra.FechaCreacion);

    public async Task<IReadOnlyList<ObraDto>> ObtenerRecientesAsync(int cantidad, CancellationToken cancelacion)
    {
        return await contexto.Obras
            .Where(obra => obra.Capitulos.Any())
            .OrderByDescending(obra => obra.Capitulos.Max(capitulo => capitulo.FechaPublicacion))
            .ThenBy(obra => obra.Id)
            .Take(cantidad)
            .Select(AObraDto)
            .ToListAsync(cancelacion);
    }

    public async Task<IReadOnlyList<ObraDto>> ObtenerTendenciasAsync(DateTime desde, int cantidad, CancellationToken cancelacion)
    {
        var idsEnOrden = await contexto.Lecturas
            .Where(lectura => lectura.FechaLectura >= desde)
            .GroupBy(lectura => lectura.Capitulo.ObraId)
            .Select(grupo => new { ObraId = grupo.Key, Total = grupo.Count() })
            .OrderByDescending(grupo => grupo.Total)
            .ThenBy(grupo => grupo.ObraId)
            .Take(cantidad)
            .Select(grupo => grupo.ObraId)
            .ToListAsync(cancelacion);

        var obras = await contexto.Obras
            .Where(obra => idsEnOrden.Contains(obra.Id))
            .Select(AObraDto)
            .ToListAsync(cancelacion);

        return idsEnOrden.Select(id => obras.Single(obra => obra.Id == id)).ToList();
    }

    public async Task<IReadOnlyList<ObraDto>> BuscarPorTituloAsync(string titulo, CancellationToken cancelacion)
    {
        var texto = titulo.ToLowerInvariant();
        return await contexto.Obras
            .Where(obra => obra.Titulo.ToLower().Contains(texto))
            .OrderBy(obra => obra.Titulo)
            .Select(AObraDto)
            .ToListAsync(cancelacion);
    }
}
