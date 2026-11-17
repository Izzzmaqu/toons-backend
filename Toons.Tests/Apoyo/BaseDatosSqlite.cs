using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Toons.Infrastructure.Persistencia;

namespace Toons.Tests.Apoyo;

public sealed class BaseDatosSqlite : IDisposable
{
    private readonly SqliteConnection conexion = new("DataSource=:memory:");

    public BaseDatosSqlite()
    {
        conexion.Open();
        using var contexto = CrearContexto();
        contexto.Database.EnsureCreated();
    }

    public ToonsDbContext CrearContexto()
    {
        var opciones = new DbContextOptionsBuilder<ToonsDbContext>()
            .UseSqlite(conexion)
            .UseSnakeCaseNamingConvention()
            .Options;
        return new ToonsDbContext(opciones);
    }

    public void Dispose()
    {
        conexion.Dispose();
    }
}
