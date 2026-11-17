using Microsoft.Extensions.Time.Testing;
using Toons.Infrastructure.Persistencia;
using Toons.Infrastructure.Seguridad;

namespace Toons.Tests.Apoyo;

public sealed class BaseSembrada : IAsyncLifetime
{
    public static readonly DateTimeOffset Ahora = new(2026, 6, 15, 12, 0, 0, TimeSpan.Zero);

    public BaseDatosSqlite BaseDatos { get; } = new();

    public FakeTimeProvider Reloj { get; } = new(Ahora);

    public async Task InitializeAsync()
    {
        await SembrarAsync(BaseDatos, Reloj);
    }

    public static async Task SembrarAsync(BaseDatosSqlite baseDatos, FakeTimeProvider reloj)
    {
        await using var contexto = baseDatos.CrearContexto();
        var sembrador = new SembradorDatos(contexto, new PasswordHasherAspNet(), reloj);
        await sembrador.SembrarAsync(CancellationToken.None);
    }

    public Task DisposeAsync()
    {
        BaseDatos.Dispose();
        return Task.CompletedTask;
    }
}
