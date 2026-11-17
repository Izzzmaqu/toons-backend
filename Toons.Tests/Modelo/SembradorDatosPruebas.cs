using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Toons.Domain.Entidades;
using Toons.Infrastructure.Persistencia;
using Toons.Infrastructure.Seguridad;
using Toons.Tests.Apoyo;

namespace Toons.Tests.Modelo;

public sealed class SembradorDatosPruebas : IAsyncLifetime
{
    private readonly BaseSembrada primera = new();

    public Task InitializeAsync() => primera.InitializeAsync();

    public Task DisposeAsync() => primera.DisposeAsync();

    private ToonsDbContext Contexto() => primera.BaseDatos.CrearContexto();

    [Fact]
    public void Siembra_las_cantidades_pedidas_de_cada_tabla()
    {
        using var contexto = Contexto();

        Assert.Equal(2, contexto.Roles.Count());
        Assert.Equal(5, contexto.Usuarios.Count());
        Assert.Equal(3, contexto.Idiomas.Count());
        Assert.Equal(8, contexto.Etiquetas.Count());
        Assert.Equal(10, contexto.Obras.Count());
        Assert.Equal(40, contexto.Capitulos.Count());
        Assert.Equal(300, contexto.Lecturas.Count());
        Assert.True(contexto.Favoritos.Any());
        Assert.True(contexto.Comentarios.Any());
        Assert.True(contexto.Calificaciones.Any());
    }

    [Fact]
    public void Cada_capitulo_tiene_entre_tres_y_cinco_paginas()
    {
        using var contexto = Contexto();

        var paginasPorCapitulo = contexto.Paginas.GroupBy(p => p.CapituloId).Select(g => g.Count()).ToList();

        Assert.Equal(40, paginasPorCapitulo.Count);
        Assert.All(paginasPorCapitulo, cantidad => Assert.InRange(cantidad, 3, 5));
    }

    [Fact]
    public void Las_obras_usan_las_portadas_01_a_10()
    {
        using var contexto = Contexto();

        var portadas = contexto.Obras.Select(o => o.Portada).ToList();

        var esperadas = Enumerable.Range(1, 10).Select(n => $"portada-{n:00}.svg");
        Assert.Equal(esperadas.Order(), portadas.Order());
    }

    [Fact]
    public async Task Los_dos_usuarios_de_ejemplo_entran_con_la_contrasena_documentada()
    {
        await using var contexto = Contexto();
        var hasher = new PasswordHasherAspNet();

        var administrador = await contexto.Usuarios.Include(u => u.Rol).SingleAsync(u => u.Correo == "admin@ejemplo.com");
        var lector = await contexto.Usuarios.Include(u => u.Rol).SingleAsync(u => u.Correo == "usuario@ejemplo.com");

        Assert.Equal("administrador", administrador.Rol.Nombre);
        Assert.True(hasher.Verificar("admin-ejemplo-123", administrador.ContrasenaHash));
        Assert.Equal("lector", lector.Rol.Nombre);
        Assert.True(hasher.Verificar("usuario-ejemplo-123", lector.ContrasenaHash));
    }

    [Fact]
    public void Las_lecturas_caen_en_los_ultimos_seis_meses_y_despues_de_publicado_el_capitulo()
    {
        using var contexto = Contexto();
        var ahora = BaseSembrada.Ahora.UtcDateTime;

        var lecturas = contexto.Lecturas.Include(l => l.Capitulo).ToList();

        Assert.All(lecturas, lectura =>
        {
            Assert.InRange(lectura.FechaLectura, ahora.AddMonths(-6), ahora);
            Assert.True(lectura.FechaLectura >= lectura.Capitulo.FechaPublicacion);
        });
    }

    [Fact]
    public void Los_datos_permiten_reportes_por_mes_y_por_creador()
    {
        using var contexto = Contexto();

        var lecturas = contexto.Lecturas.Include(l => l.Capitulo).ThenInclude(c => c.Obra).ToList();

        Assert.True(lecturas.Select(l => (l.FechaLectura.Year, l.FechaLectura.Month)).Distinct().Count() >= 5);
        Assert.True(lecturas.Select(l => l.Capitulo.Obra.CreadorId).Distinct().Count() >= 3);
    }

    [Fact]
    public async Task Sembrar_de_nuevo_no_duplica_nada()
    {
        await BaseSembrada.SembrarAsync(primera.BaseDatos, primera.Reloj);

        using var contexto = Contexto();
        Assert.Equal(10, contexto.Obras.Count());
        Assert.Equal(300, contexto.Lecturas.Count());
        Assert.Equal(5, contexto.Usuarios.Count());
    }

    [Fact]
    public async Task Con_el_mismo_reloj_siempre_siembra_exactamente_lo_mismo()
    {
        using var otraBase = new BaseDatosSqlite();
        await BaseSembrada.SembrarAsync(otraBase, new FakeTimeProvider(BaseSembrada.Ahora));

        Assert.Equal(Huella(primera.BaseDatos), Huella(otraBase));
    }

    private static List<string> Huella(BaseDatosSqlite baseDatos)
    {
        using var contexto = baseDatos.CrearContexto();
        var lecturas = contexto.Lecturas.OrderBy(l => l.Id)
            .Select(l => $"L {l.UsuarioId} {l.CapituloId} {l.FechaLectura:O}").ToList();
        var calificaciones = contexto.Calificaciones.OrderBy(c => c.Id)
            .Select(c => $"C {c.UsuarioId} {c.ObraId} {c.Puntaje} {c.Fecha:O}").ToList();
        var comentarios = contexto.Comentarios.OrderBy(c => c.Id)
            .Select(c => $"M {c.UsuarioId} {c.ObraId} {c.Texto}").ToList();
        var favoritos = contexto.Favoritos.OrderBy(f => f.UsuarioId).ThenBy(f => f.ObraId)
            .Select(f => $"F {f.UsuarioId} {f.ObraId}").ToList();
        var paginas = contexto.Paginas.OrderBy(p => p.Id)
            .Select(p => $"P {p.CapituloId} {p.Numero}").ToList();
        return lecturas.Concat(calificaciones).Concat(comentarios).Concat(favoritos).Concat(paginas).ToList();
    }
}
