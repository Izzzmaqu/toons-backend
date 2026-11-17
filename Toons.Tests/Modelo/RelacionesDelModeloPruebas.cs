using Microsoft.EntityFrameworkCore;
using Toons.Domain.Entidades;
using Toons.Tests.Apoyo;
using static Toons.Tests.Apoyo.ConstructorDeDatos;

namespace Toons.Tests.Modelo;

public sealed class RelacionesDelModeloPruebas : IDisposable
{
    private readonly BaseDatosSqlite baseDatos = new();

    [Fact]
    public void Las_relaciones_de_una_obra_se_cargan_con_Include()
    {
        int obraId;
        using (var escritura = baseDatos.CrearContexto())
        {
            var rol = NuevoRol(escritura);
            var creador = NuevoUsuario(escritura, rol);
            var idioma = NuevoIdioma(escritura, "Coreano");
            var etiqueta = NuevaEtiqueta(escritura, "Fantasía");
            var obra = NuevaObra(escritura, creador, idioma);
            var capitulo = NuevoCapitulo(escritura, obra);
            NuevaPagina(escritura, capitulo, 1);
            NuevaPagina(escritura, capitulo, 2);
            escritura.ObrasEtiquetas.Add(new ObraEtiqueta { ObraId = obra.Id, EtiquetaId = etiqueta.Id });
            escritura.SaveChanges();
            obraId = obra.Id;
        }

        using var lectura = baseDatos.CrearContexto();
        var cargada = lectura.Obras
            .Include(o => o.Creador).ThenInclude(u => u.Rol)
            .Include(o => o.Idioma)
            .Include(o => o.ObrasEtiquetas).ThenInclude(oe => oe.Etiqueta)
            .Include(o => o.Capitulos).ThenInclude(c => c.Paginas)
            .Single(o => o.Id == obraId);

        Assert.Equal("lector", cargada.Creador.Rol.Nombre);
        Assert.Equal("Coreano", cargada.Idioma.Nombre);
        Assert.Equal("Fantasía", Assert.Single(cargada.ObrasEtiquetas).Etiqueta.Nombre);
        Assert.Equal(2, Assert.Single(cargada.Capitulos).Paginas.Count);
    }

    [Fact]
    public void Las_relaciones_de_un_usuario_se_cargan_con_Include()
    {
        int usuarioId;
        using (var escritura = baseDatos.CrearContexto())
        {
            var rol = NuevoRol(escritura);
            var usuario = NuevoUsuario(escritura, rol);
            var obra = NuevaObra(escritura, usuario, NuevoIdioma(escritura));
            var capitulo = NuevoCapitulo(escritura, obra);
            NuevaLectura(escritura, usuario, capitulo, DateTime.UtcNow);
            escritura.Favoritos.Add(new Favorito { UsuarioId = usuario.Id, ObraId = obra.Id });
            escritura.Comentarios.Add(new Comentario { UsuarioId = usuario.Id, ObraId = obra.Id, Texto = "Me gustó" });
            escritura.Calificaciones.Add(new Calificacion { UsuarioId = usuario.Id, ObraId = obra.Id, Puntaje = 5 });
            escritura.SaveChanges();
            usuarioId = usuario.Id;
        }

        using var lectura = baseDatos.CrearContexto();
        var cargado = lectura.Usuarios
            .Include(u => u.ObrasCreadas)
            .Include(u => u.Favoritos).ThenInclude(f => f.Obra)
            .Include(u => u.Lecturas).ThenInclude(l => l.Capitulo)
            .Include(u => u.Comentarios)
            .Include(u => u.Calificaciones)
            .Single(u => u.Id == usuarioId);

        Assert.Single(cargado.ObrasCreadas);
        Assert.Equal(cargado.ObrasCreadas.Single().Id, Assert.Single(cargado.Favoritos).Obra.Id);
        Assert.Equal(1, Assert.Single(cargado.Lecturas).Capitulo.Numero);
        Assert.Equal("Me gustó", Assert.Single(cargado.Comentarios).Texto);
        Assert.Equal(5, Assert.Single(cargado.Calificaciones).Puntaje);
    }

    public void Dispose()
    {
        baseDatos.Dispose();
    }
}
