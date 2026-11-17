using Microsoft.EntityFrameworkCore;
using Toons.Domain.Entidades;
using Toons.Infrastructure.Persistencia;
using Toons.Tests.Apoyo;
using static Toons.Tests.Apoyo.ConstructorDeDatos;

namespace Toons.Tests.Modelo;

public sealed class RestriccionesDelModeloPruebas : IDisposable
{
    private readonly BaseDatosSqlite baseDatos = new();
    private readonly ToonsDbContext contexto;
    private readonly Rol rol;
    private readonly Usuario usuario;
    private readonly Idioma idioma;
    private readonly Obra obra;
    private readonly Capitulo capitulo;

    public RestriccionesDelModeloPruebas()
    {
        contexto = baseDatos.CrearContexto();
        rol = NuevoRol(contexto);
        usuario = NuevoUsuario(contexto, rol);
        idioma = NuevoIdioma(contexto);
        obra = NuevaObra(contexto, usuario, idioma);
        capitulo = NuevoCapitulo(contexto, obra);
        contexto.ChangeTracker.Clear();
    }

    private void MarcarParaBorrarSoloLaFila<T>(int id) where T : class
    {
        contexto.ChangeTracker.Clear();
        contexto.Remove(contexto.Find<T>(id)!);
    }

    private void AfirmarQueLaBaseRechaza(Action accion)
    {
        Assert.Throws<DbUpdateException>(accion);
        contexto.ChangeTracker.Clear();
    }

    [Fact]
    public void El_correo_del_usuario_es_unico()
    {
        AfirmarQueLaBaseRechaza(() => NuevoUsuario(contexto, rol, usuario.Correo));
    }

    [Fact]
    public void Los_nombres_de_rol_idioma_y_etiqueta_son_unicos()
    {
        NuevaEtiqueta(contexto, "Drama");

        AfirmarQueLaBaseRechaza(() => NuevoRol(contexto, rol.Nombre));
        AfirmarQueLaBaseRechaza(() => NuevoIdioma(contexto, idioma.Nombre));
        AfirmarQueLaBaseRechaza(() => NuevaEtiqueta(contexto, "Drama"));
    }

    [Fact]
    public void El_numero_de_capitulo_es_unico_dentro_de_la_obra()
    {
        AfirmarQueLaBaseRechaza(() => NuevoCapitulo(contexto, obra, capitulo.Numero));
    }

    [Fact]
    public void El_mismo_numero_de_capitulo_se_permite_en_otra_obra()
    {
        var otraObra = NuevaObra(contexto, usuario, idioma, "Otra obra");

        var capituloEnOtraObra = NuevoCapitulo(contexto, otraObra, capitulo.Numero);

        Assert.NotEqual(capitulo.Id, capituloEnOtraObra.Id);
    }

    [Fact]
    public void El_numero_de_pagina_es_unico_dentro_del_capitulo()
    {
        var pagina = NuevaPagina(contexto, capitulo);

        AfirmarQueLaBaseRechaza(() => NuevaPagina(contexto, capitulo, pagina.Numero));
    }

    [Fact]
    public void Un_usuario_solo_puede_calificar_una_vez_cada_obra()
    {
        contexto.Calificaciones.Add(new Calificacion { UsuarioId = usuario.Id, ObraId = obra.Id, Puntaje = 4 });
        contexto.SaveChanges();

        contexto.Calificaciones.Add(new Calificacion { UsuarioId = usuario.Id, ObraId = obra.Id, Puntaje = 5 });

        AfirmarQueLaBaseRechaza(() => contexto.SaveChanges());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void El_puntaje_entre_uno_y_cinco_se_acepta(int puntaje)
    {
        contexto.Calificaciones.Add(new Calificacion { UsuarioId = usuario.Id, ObraId = obra.Id, Puntaje = puntaje });

        contexto.SaveChanges();

        Assert.Equal(puntaje, contexto.Calificaciones.Single().Puntaje);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void El_puntaje_fuera_de_uno_a_cinco_lo_rechaza_la_base(int puntaje)
    {
        contexto.Calificaciones.Add(new Calificacion { UsuarioId = usuario.Id, ObraId = obra.Id, Puntaje = puntaje });

        AfirmarQueLaBaseRechaza(() => contexto.SaveChanges());
    }

    [Fact]
    public void La_clave_compuesta_de_obras_etiquetas_impide_duplicados()
    {
        var etiqueta = NuevaEtiqueta(contexto);
        contexto.ObrasEtiquetas.Add(new ObraEtiqueta { ObraId = obra.Id, EtiquetaId = etiqueta.Id });
        contexto.SaveChanges();
        contexto.ChangeTracker.Clear();

        contexto.ObrasEtiquetas.Add(new ObraEtiqueta { ObraId = obra.Id, EtiquetaId = etiqueta.Id });

        AfirmarQueLaBaseRechaza(() => contexto.SaveChanges());
    }

    [Fact]
    public void La_clave_compuesta_de_favoritos_impide_duplicados()
    {
        contexto.Favoritos.Add(new Favorito { UsuarioId = usuario.Id, ObraId = obra.Id });
        contexto.SaveChanges();
        contexto.ChangeTracker.Clear();

        contexto.Favoritos.Add(new Favorito { UsuarioId = usuario.Id, ObraId = obra.Id });

        AfirmarQueLaBaseRechaza(() => contexto.SaveChanges());
    }

    [Fact]
    public void Una_obra_puede_tener_varias_etiquetas_y_una_etiqueta_varias_obras()
    {
        var accion = NuevaEtiqueta(contexto, "Acción");
        var drama = NuevaEtiqueta(contexto, "Drama");
        var otraObra = NuevaObra(contexto, usuario, idioma, "Otra obra");
        contexto.ObrasEtiquetas.AddRange(
            new ObraEtiqueta { ObraId = obra.Id, EtiquetaId = accion.Id },
            new ObraEtiqueta { ObraId = obra.Id, EtiquetaId = drama.Id },
            new ObraEtiqueta { ObraId = otraObra.Id, EtiquetaId = accion.Id });

        contexto.SaveChanges();

        Assert.Equal(3, contexto.ObrasEtiquetas.Count());
    }

    [Fact]
    public void No_se_puede_borrar_un_usuario_que_creo_obras()
    {
        MarcarParaBorrarSoloLaFila<Usuario>(usuario.Id);

        AfirmarQueLaBaseRechaza(() => contexto.SaveChanges());
    }

    [Fact]
    public void No_se_puede_borrar_un_rol_con_usuarios()
    {
        MarcarParaBorrarSoloLaFila<Rol>(rol.Id);

        AfirmarQueLaBaseRechaza(() => contexto.SaveChanges());
    }

    [Fact]
    public void No_se_puede_borrar_un_idioma_con_obras()
    {
        MarcarParaBorrarSoloLaFila<Idioma>(idioma.Id);

        AfirmarQueLaBaseRechaza(() => contexto.SaveChanges());
    }

    [Fact]
    public void No_se_puede_borrar_un_capitulo_con_lecturas()
    {
        NuevaLectura(contexto, usuario, capitulo, DateTime.UtcNow);
        MarcarParaBorrarSoloLaFila<Capitulo>(capitulo.Id);

        AfirmarQueLaBaseRechaza(() => contexto.SaveChanges());
    }

    [Fact]
    public void No_se_puede_borrar_una_obra_con_comentarios()
    {
        contexto.Comentarios.Add(new Comentario { UsuarioId = usuario.Id, ObraId = obra.Id, Texto = "Muy buena" });
        contexto.SaveChanges();
        MarcarParaBorrarSoloLaFila<Obra>(obra.Id);

        AfirmarQueLaBaseRechaza(() => contexto.SaveChanges());
    }

    [Fact]
    public void No_se_puede_borrar_una_obra_con_calificaciones()
    {
        contexto.Calificaciones.Add(new Calificacion { UsuarioId = usuario.Id, ObraId = obra.Id, Puntaje = 5 });
        contexto.SaveChanges();
        MarcarParaBorrarSoloLaFila<Obra>(obra.Id);

        AfirmarQueLaBaseRechaza(() => contexto.SaveChanges());
    }

    [Fact]
    public void Borrar_un_capitulo_borra_sus_paginas()
    {
        NuevaPagina(contexto, capitulo);
        contexto.Capitulos.Remove(capitulo);

        contexto.SaveChanges();

        Assert.Empty(contexto.Paginas);
    }

    [Fact]
    public void Borrar_una_obra_borra_sus_capitulos_paginas_etiquetas_y_favoritos()
    {
        NuevaPagina(contexto, capitulo);
        var etiqueta = NuevaEtiqueta(contexto);
        contexto.ObrasEtiquetas.Add(new ObraEtiqueta { ObraId = obra.Id, EtiquetaId = etiqueta.Id });
        contexto.Favoritos.Add(new Favorito { UsuarioId = usuario.Id, ObraId = obra.Id });
        contexto.SaveChanges();
        contexto.Obras.Remove(obra);

        contexto.SaveChanges();

        Assert.Empty(contexto.Capitulos);
        Assert.Empty(contexto.Paginas);
        Assert.Empty(contexto.ObrasEtiquetas);
        Assert.Empty(contexto.Favoritos);
        Assert.Single(contexto.Etiquetas);
    }

    [Fact]
    public void Borrar_una_etiqueta_borra_solo_sus_filas_de_la_tabla_puente()
    {
        var etiqueta = NuevaEtiqueta(contexto);
        contexto.ObrasEtiquetas.Add(new ObraEtiqueta { ObraId = obra.Id, EtiquetaId = etiqueta.Id });
        contexto.SaveChanges();
        contexto.Etiquetas.Remove(etiqueta);

        contexto.SaveChanges();

        Assert.Empty(contexto.ObrasEtiquetas);
        Assert.Single(contexto.Obras);
    }

    public void Dispose()
    {
        contexto.Dispose();
        baseDatos.Dispose();
    }
}
