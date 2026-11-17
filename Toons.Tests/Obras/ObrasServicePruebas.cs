using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Toons.Application.Obras;
using Toons.Infrastructure.Persistencia;
using Toons.Tests.Apoyo;
using static Toons.Tests.Apoyo.ConstructorDeDatos;

namespace Toons.Tests.Obras;

public sealed class ObrasServicePruebas : IClassFixture<BaseSembrada>
{
    private const int TodasLasObras = 10;
    private readonly BaseSembrada baseSembrada;

    public ObrasServicePruebas(BaseSembrada baseSembrada)
    {
        this.baseSembrada = baseSembrada;
    }

    private static ObrasService CrearServicio(ToonsDbContext contexto, FakeTimeProvider reloj)
    {
        return new ObrasService(new ObraRepositorio(contexto), reloj);
    }

    private ObrasService ServicioSobreLaBaseSembrada(out ToonsDbContext contexto)
    {
        contexto = baseSembrada.BaseDatos.CrearContexto();
        return CrearServicio(contexto, baseSembrada.Reloj);
    }

    [Fact]
    public async Task Recientes_ordena_por_la_fecha_del_ultimo_capitulo_publicado()
    {
        var servicio = ServicioSobreLaBaseSembrada(out var contexto);
        using var _ = contexto;

        var recientes = await servicio.ObtenerRecientesAsync(TodasLasObras, CancellationToken.None);

        var esperadas = contexto.Capitulos
            .GroupBy(c => c.ObraId)
            .Select(g => new { ObraId = g.Key, Ultima = g.Max(c => c.FechaPublicacion) })
            .AsEnumerable()
            .OrderByDescending(x => x.Ultima)
            .Select(x => x.ObraId)
            .ToList();
        Assert.Equal(esperadas, recientes.Select(o => o.Id));
        Assert.Equal(recientes.Select(o => o.FechaPublicacion).Order().Reverse(), recientes.Select(o => o.FechaPublicacion));
    }

    [Fact]
    public async Task Recientes_respeta_la_cantidad_pedida()
    {
        var servicio = ServicioSobreLaBaseSembrada(out var contexto);
        using var _ = contexto;

        var recientes = await servicio.ObtenerRecientesAsync(3, CancellationToken.None);

        Assert.Equal(3, recientes.Count);
    }

    [Fact]
    public async Task Cada_obra_trae_titulo_portada_y_nombre_del_creador()
    {
        var servicio = ServicioSobreLaBaseSembrada(out var contexto);
        using var _ = contexto;

        var obras = await servicio.ObtenerRecientesAsync(TodasLasObras, CancellationToken.None);

        Assert.Equal(TodasLasObras, obras.Count);
        Assert.All(obras, obra =>
        {
            Assert.False(string.IsNullOrWhiteSpace(obra.Titulo));
            Assert.Matches(@"^portada-\d\d\.svg$", obra.Portada);
            Assert.False(string.IsNullOrWhiteSpace(obra.Creador));
        });
    }

    [Fact]
    public async Task Tendencias_ordena_por_lecturas_de_los_ultimos_treinta_dias()
    {
        var servicio = ServicioSobreLaBaseSembrada(out var contexto);
        using var _ = contexto;
        var desde = BaseSembrada.Ahora.UtcDateTime.AddDays(-30);

        var tendencias = await servicio.ObtenerTendenciasAsync(TodasLasObras, CancellationToken.None);

        var esperadas = contexto.Lecturas
            .Where(l => l.FechaLectura >= desde)
            .Select(l => l.Capitulo.ObraId)
            .AsEnumerable()
            .GroupBy(obraId => obraId)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key)
            .Select(g => g.Key)
            .ToList();
        Assert.NotEmpty(esperadas);
        Assert.Equal(esperadas, tendencias.Select(o => o.Id));
    }

    [Fact]
    public async Task Tendencias_ignora_las_lecturas_de_hace_mas_de_treinta_dias_y_las_obras_sin_lecturas()
    {
        using var baseDatos = new BaseDatosSqlite();
        using var contexto = baseDatos.CrearContexto();
        var reloj = new FakeTimeProvider(BaseSembrada.Ahora);
        var ahora = reloj.GetUtcNow().UtcDateTime;
        var usuario = NuevoUsuario(contexto, NuevoRol(contexto));
        var idioma = NuevoIdioma(contexto);
        var obraAntigua = NuevaObra(contexto, usuario, idioma, "Lecturas viejas");
        var obraActual = NuevaObra(contexto, usuario, idioma, "Lecturas recientes");
        NuevaObra(contexto, usuario, idioma, "Sin lecturas");
        var capituloAntiguo = NuevoCapitulo(contexto, obraAntigua, 1, ahora.AddDays(-90));
        var capituloActual = NuevoCapitulo(contexto, obraActual, 1, ahora.AddDays(-90));
        for (var i = 0; i < 3; i++)
        {
            NuevaLectura(contexto, usuario, capituloAntiguo, ahora.AddDays(-40 - i));
        }

        NuevaLectura(contexto, usuario, capituloActual, ahora.AddDays(-2));

        var tendencias = await CrearServicio(contexto, reloj).ObtenerTendenciasAsync(TodasLasObras, CancellationToken.None);

        Assert.Equal(["Lecturas recientes"], tendencias.Select(o => o.Titulo));
    }

    [Theory]
    [InlineData("torre")]
    [InlineData("TORRE")]
    [InlineData("  Torre de Cr  ")]
    public async Task Buscar_encuentra_por_titulo_sin_distinguir_mayusculas(string texto)
    {
        var servicio = ServicioSobreLaBaseSembrada(out var contexto);
        using var _ = contexto;

        var resultados = await servicio.BuscarPorTituloAsync(texto, CancellationToken.None);

        Assert.Equal("La Torre de Cristal", Assert.Single(resultados).Titulo);
    }

    [Fact]
    public async Task Buscar_sin_coincidencias_devuelve_lista_vacia()
    {
        var servicio = ServicioSobreLaBaseSembrada(out var contexto);
        using var _ = contexto;

        var resultados = await servicio.BuscarPorTituloAsync("texto que no existe", CancellationToken.None);

        Assert.Empty(resultados);
    }

    [Fact]
    public async Task Buscar_trata_el_porcentaje_como_texto_y_no_como_comodin()
    {
        var servicio = ServicioSobreLaBaseSembrada(out var contexto);
        using var _ = contexto;

        var resultados = await servicio.BuscarPorTituloAsync("%", CancellationToken.None);

        Assert.Empty(resultados);
    }

    [Fact]
    public async Task Buscar_con_texto_vacio_devuelve_todas_las_obras()
    {
        var servicio = ServicioSobreLaBaseSembrada(out var contexto);
        using var _ = contexto;

        var resultados = await servicio.BuscarPorTituloAsync("   ", CancellationToken.None);

        Assert.Equal(TodasLasObras, resultados.Count);
    }
}
