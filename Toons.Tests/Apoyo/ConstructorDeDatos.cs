using Toons.Domain.Entidades;
using Toons.Infrastructure.Persistencia;

namespace Toons.Tests.Apoyo;

public static class ConstructorDeDatos
{
    private static readonly DateTime FechaBase = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static Rol NuevoRol(ToonsDbContext contexto, string nombre = "lector")
    {
        return Guardar(contexto, new Rol { Nombre = nombre });
    }

    public static Usuario NuevoUsuario(ToonsDbContext contexto, Rol rol, string correo = "persona@ejemplo.com")
    {
        return Guardar(contexto, new Usuario
        {
            Nombre = "Persona de prueba",
            Correo = correo,
            ContrasenaHash = "hash",
            RolId = rol.Id,
            FechaRegistro = FechaBase,
        });
    }

    public static Idioma NuevoIdioma(ToonsDbContext contexto, string nombre = "Español")
    {
        return Guardar(contexto, new Idioma { Nombre = nombre });
    }

    public static Etiqueta NuevaEtiqueta(ToonsDbContext contexto, string nombre = "Acción")
    {
        return Guardar(contexto, new Etiqueta { Nombre = nombre });
    }

    public static Obra NuevaObra(ToonsDbContext contexto, Usuario creador, Idioma idioma, string titulo = "Obra de prueba")
    {
        return Guardar(contexto, new Obra
        {
            Titulo = titulo,
            Descripcion = "Descripción de prueba",
            Portada = "portada-01.svg",
            CreadorId = creador.Id,
            IdiomaId = idioma.Id,
            FechaCreacion = FechaBase,
        });
    }

    public static Capitulo NuevoCapitulo(ToonsDbContext contexto, Obra obra, int numero = 1, DateTime? fechaPublicacion = null)
    {
        return Guardar(contexto, new Capitulo
        {
            ObraId = obra.Id,
            Numero = numero,
            Titulo = $"Capítulo {numero}",
            FechaPublicacion = fechaPublicacion ?? FechaBase,
        });
    }

    public static Pagina NuevaPagina(ToonsDbContext contexto, Capitulo capitulo, int numero = 1)
    {
        return Guardar(contexto, new Pagina
        {
            CapituloId = capitulo.Id,
            Numero = numero,
            UrlImagen = $"paginas/{capitulo.Id}/{numero}.webp",
        });
    }

    public static Lectura NuevaLectura(ToonsDbContext contexto, Usuario usuario, Capitulo capitulo, DateTime fechaLectura)
    {
        return Guardar(contexto, new Lectura
        {
            UsuarioId = usuario.Id,
            CapituloId = capitulo.Id,
            FechaLectura = fechaLectura,
        });
    }

    private static T Guardar<T>(ToonsDbContext contexto, T entidad) where T : class
    {
        contexto.Add(entidad);
        contexto.SaveChanges();
        return entidad;
    }
}
