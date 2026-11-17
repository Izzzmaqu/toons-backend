namespace Toons.Application.Obras;

public sealed record ObraDto(
    int Id,
    string Titulo,
    string Descripcion,
    string Portada,
    string Creador,
    DateTime FechaPublicacion);
