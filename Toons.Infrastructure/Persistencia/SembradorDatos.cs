using Microsoft.EntityFrameworkCore;
using Toons.Application.Autenticacion;
using Toons.Domain.Entidades;

namespace Toons.Infrastructure.Persistencia;

public class SembradorDatos(ToonsDbContext contexto, IPasswordHasher hasher, TimeProvider reloj)
{
    private const int DiasDeHistoria = 180;
    private const int CapitulosPorObra = 4;
    private const int DiasEntreCapitulos = 7;
    private const int DiasEntreObras = 17;
    private const int CantidadDeLecturas = 300;
    private const int CantidadDeComentarios = 25;
    private const int FavoritosPorUsuario = 3;
    private const int CalificacionesPorUsuario = 5;

    public async Task SembrarAsync(CancellationToken cancelacion)
    {
        if (await contexto.Roles.AnyAsync(cancelacion))
        {
            return;
        }

        var ahora = reloj.GetUtcNow().UtcDateTime;
        var azar = new Random(CatalogoDeEjemplo.Semilla);
        var usuarios = CrearUsuarios(ahora);
        var creadores = usuarios.Skip(1).ToList();
        var idiomas = CatalogoDeEjemplo.Idiomas.Select(nombre => new Idioma { Nombre = nombre }).ToList();
        var etiquetas = CatalogoDeEjemplo.Etiquetas.Select(nombre => new Etiqueta { Nombre = nombre }).ToList();
        var obras = CrearObras(creadores, idiomas, etiquetas, ahora, azar);

        contexto.AddRange(usuarios);
        contexto.AddRange(idiomas);
        contexto.AddRange(etiquetas);
        contexto.AddRange(obras);
        contexto.AddRange(CrearLecturas(usuarios, obras, ahora, azar));
        contexto.AddRange(CrearFavoritos(usuarios, obras, ahora, azar));
        contexto.AddRange(CrearComentarios(usuarios, obras, ahora, azar));
        contexto.AddRange(CrearCalificaciones(usuarios, obras, ahora, azar));
        await contexto.SaveChangesAsync(cancelacion);
    }

    private List<Usuario> CrearUsuarios(DateTime ahora)
    {
        var administrador = new Rol { Nombre = "administrador" };
        var lector = new Rol { Nombre = "lector" };
        return
        [
            CrearUsuario("Admin de ejemplo", "admin@ejemplo.com", CatalogoDeEjemplo.ContrasenaAdministrador, administrador, ahora),
            CrearUsuario("Usuario de ejemplo", "usuario@ejemplo.com", CatalogoDeEjemplo.ContrasenaUsuario, lector, ahora),
            CrearUsuario("Lucía Torres", "lucia@ejemplo.com", CatalogoDeEjemplo.ContrasenaLectores, lector, ahora),
            CrearUsuario("Mateo Ríos", "mateo@ejemplo.com", CatalogoDeEjemplo.ContrasenaLectores, lector, ahora),
            CrearUsuario("Sofía Vega", "sofia@ejemplo.com", CatalogoDeEjemplo.ContrasenaLectores, lector, ahora),
        ];
    }

    private Usuario CrearUsuario(string nombre, string correo, string contrasena, Rol rol, DateTime ahora)
    {
        return new Usuario
        {
            Nombre = nombre,
            Correo = correo,
            ContrasenaHash = hasher.Cifrar(contrasena),
            Rol = rol,
            FechaRegistro = ahora.AddDays(-DiasDeHistoria),
        };
    }

    private static List<Obra> CrearObras(
        List<Usuario> creadores, List<Idioma> idiomas, List<Etiqueta> etiquetas, DateTime ahora, Random azar)
    {
        var obras = new List<Obra>();
        for (var indice = 0; indice < CatalogoDeEjemplo.Obras.Length; indice++)
        {
            var (titulo, descripcion) = CatalogoDeEjemplo.Obras[indice];
            var ultimaPublicacion = ahora.AddDays(-(3 + indice * DiasEntreObras));
            var primeraPublicacion = ultimaPublicacion.AddDays(-(CapitulosPorObra - 1) * DiasEntreCapitulos);
            var obra = new Obra
            {
                Titulo = titulo,
                Descripcion = descripcion,
                Portada = $"portada-{indice + 1:00}.svg",
                Creador = creadores[indice % creadores.Count],
                Idioma = idiomas[indice % idiomas.Count],
                FechaCreacion = primeraPublicacion.AddDays(-5),
            };
            AgregarEtiquetas(obra, etiquetas, azar);
            AgregarCapitulos(obra, primeraPublicacion, azar);
            obras.Add(obra);
        }

        return obras;
    }

    private static void AgregarEtiquetas(Obra obra, List<Etiqueta> etiquetas, Random azar)
    {
        var primera = azar.Next(etiquetas.Count);
        var cantidad = azar.Next(2, 4);
        for (var desplazamiento = 0; desplazamiento < cantidad; desplazamiento++)
        {
            var etiqueta = etiquetas[(primera + desplazamiento) % etiquetas.Count];
            obra.ObrasEtiquetas.Add(new ObraEtiqueta { Obra = obra, Etiqueta = etiqueta });
        }
    }

    private static void AgregarCapitulos(Obra obra, DateTime primeraPublicacion, Random azar)
    {
        for (var numero = 1; numero <= CapitulosPorObra; numero++)
        {
            var capitulo = new Capitulo
            {
                Obra = obra,
                Numero = numero,
                Titulo = $"Capítulo {numero}",
                FechaPublicacion = primeraPublicacion.AddDays((numero - 1) * DiasEntreCapitulos),
            };
            AgregarPaginas(capitulo, azar.Next(3, 6));
            obra.Capitulos.Add(capitulo);
        }
    }

    private static void AgregarPaginas(Capitulo capitulo, int cantidad)
    {
        for (var numero = 1; numero <= cantidad; numero++)
        {
            capitulo.Paginas.Add(new Pagina
            {
                Capitulo = capitulo,
                Numero = numero,
                UrlImagen = $"paginas/{capitulo.Obra.Portada}/capitulo-{capitulo.Numero:00}/pagina-{numero:00}.webp",
            });
        }
    }

    private static List<Lectura> CrearLecturas(List<Usuario> usuarios, List<Obra> obras, DateTime ahora, Random azar)
    {
        var capitulos = obras.SelectMany(obra => obra.Capitulos).ToList();
        var inicioDeHistoria = ahora.AddDays(-DiasDeHistoria);
        return Enumerable.Range(0, CantidadDeLecturas).Select(_ =>
        {
            var capitulo = capitulos[azar.Next(capitulos.Count)];
            var desde = capitulo.FechaPublicacion > inicioDeHistoria ? capitulo.FechaPublicacion : inicioDeHistoria;
            return new Lectura
            {
                Usuario = usuarios[azar.Next(usuarios.Count)],
                Capitulo = capitulo,
                FechaLectura = FechaAleatoria(azar, desde, ahora),
            };
        }).ToList();
    }

    private static List<Favorito> CrearFavoritos(List<Usuario> usuarios, List<Obra> obras, DateTime ahora, Random azar)
    {
        return usuarios.SelectMany(usuario => ElegirObras(obras, FavoritosPorUsuario, azar).Select(obra => new Favorito
        {
            Usuario = usuario,
            Obra = obra,
            FechaMarcado = FechaAleatoria(azar, ahora.AddDays(-DiasDeHistoria), ahora),
        })).ToList();
    }

    private static List<Comentario> CrearComentarios(List<Usuario> usuarios, List<Obra> obras, DateTime ahora, Random azar)
    {
        return Enumerable.Range(0, CantidadDeComentarios).Select(_ => new Comentario
        {
            Usuario = usuarios[azar.Next(usuarios.Count)],
            Obra = obras[azar.Next(obras.Count)],
            Texto = CatalogoDeEjemplo.Comentarios[azar.Next(CatalogoDeEjemplo.Comentarios.Length)],
            Fecha = FechaAleatoria(azar, ahora.AddDays(-DiasDeHistoria), ahora),
        }).ToList();
    }

    private static List<Calificacion> CrearCalificaciones(List<Usuario> usuarios, List<Obra> obras, DateTime ahora, Random azar)
    {
        return usuarios.SelectMany(usuario => ElegirObras(obras, CalificacionesPorUsuario, azar).Select(obra => new Calificacion
        {
            Usuario = usuario,
            Obra = obra,
            Puntaje = azar.Next(1, 6),
            Fecha = FechaAleatoria(azar, ahora.AddDays(-DiasDeHistoria), ahora),
        })).ToList();
    }

    private static List<Obra> ElegirObras(List<Obra> obras, int cantidad, Random azar)
    {
        return obras.OrderBy(_ => azar.Next()).Take(cantidad).ToList();
    }

    private static DateTime FechaAleatoria(Random azar, DateTime desde, DateTime hasta)
    {
        var minutos = (hasta - desde).TotalMinutes;
        return desde.AddMinutes(Math.Floor(azar.NextDouble() * minutos));
    }
}
