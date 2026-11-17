namespace Toons.Infrastructure.Persistencia;

internal static class CatalogoDeEjemplo
{
    public const int Semilla = 20260101;

    public const string ContrasenaAdministrador = "admin-ejemplo-123";
    public const string ContrasenaUsuario = "usuario-ejemplo-123";
    public const string ContrasenaLectores = "lector-ejemplo-123";

    public static readonly string[] Idiomas = ["Español", "Inglés", "Coreano"];

    public static readonly string[] Etiquetas =
    [
        "Acción", "Aventura", "Comedia", "Drama", "Fantasía", "Romance", "Misterio", "Ciencia ficción",
    ];

    public static readonly (string Titulo, string Descripcion)[] Obras =
    [
        ("La Torre de Cristal", "Una joven cartógrafa descubre una torre que aparece solo durante las tormentas."),
        ("Sombras de Neón", "En una ciudad que nunca duerme, un mensajero nocturno persigue un secreto luminoso."),
        ("El Jardín Errante", "Un jardín que cambia de lugar cada noche guarda la memoria de un pueblo entero."),
        ("Cenizas del Alba", "Tras el último incendio, una herrera reconstruye el reino con sus propias manos."),
        ("Ruta de los Faroles", "Dos hermanos recorren un camino iluminado por faroles que cuentan historias."),
        ("Mar de Papel", "Un barco hecho de cartas viejas navega hacia el puerto donde nacen las promesas."),
        ("Los Guardianes del Eco", "Cuatro amigas protegen las voces perdidas que viven dentro de las montañas."),
        ("Café a Medianoche", "En una cafetería abierta solo de madrugada, cada cliente trae un misterio distinto."),
        ("Dragón de Bolsillo", "Un dragón del tamaño de una moneda se esconde en la mochila de un estudiante."),
        ("Estación Cero", "Una estación espacial abandonada despierta cuando llega su nueva ingeniera."),
    ];

    public static readonly string[] Comentarios =
    [
        "Me encantó este capítulo.",
        "El final me dejó con ganas de más.",
        "Los dibujos son preciosos.",
        "Ojalá publiquen el siguiente pronto.",
        "Un giro que no esperaba.",
        "Muy buena historia, la recomiendo.",
    ];
}
