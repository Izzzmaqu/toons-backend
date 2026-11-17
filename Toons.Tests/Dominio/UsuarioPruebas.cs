using Toons.Domain.Entidades;

namespace Toons.Tests.Dominio;

public class UsuarioPruebas
{
    private const int MaximoIntentos = 5;
    private static readonly TimeSpan DuracionBloqueo = TimeSpan.FromMinutes(5);
    private static readonly DateTime Ahora = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    private static Usuario CrearUsuario() => new()
    {
        Nombre = "Persona de prueba",
        Correo = "prueba@ejemplo.com",
        ContrasenaHash = "hash",
    };

    private static void FallarVeces(Usuario usuario, int veces, DateTime ahora)
    {
        for (var i = 0; i < veces; i++)
        {
            usuario.RegistrarIntentoFallido(ahora, MaximoIntentos, DuracionBloqueo);
        }
    }

    [Fact]
    public void Usuario_nuevo_no_esta_bloqueado()
    {
        var usuario = CrearUsuario();

        Assert.False(usuario.EstaBloqueado(Ahora));
        Assert.Equal(0, usuario.IntentosFallidos);
    }

    [Fact]
    public void Un_intento_fallido_suma_uno_y_no_bloquea()
    {
        var usuario = CrearUsuario();

        FallarVeces(usuario, 1, Ahora);

        Assert.Equal(1, usuario.IntentosFallidos);
        Assert.False(usuario.EstaBloqueado(Ahora));
    }

    [Fact]
    public void Con_cuatro_intentos_fallidos_todavia_no_esta_bloqueado()
    {
        var usuario = CrearUsuario();

        FallarVeces(usuario, MaximoIntentos - 1, Ahora);

        Assert.False(usuario.EstaBloqueado(Ahora));
    }

    [Fact]
    public void El_quinto_intento_fallido_bloquea_por_cinco_minutos()
    {
        var usuario = CrearUsuario();

        FallarVeces(usuario, MaximoIntentos, Ahora);

        Assert.True(usuario.EstaBloqueado(Ahora));
        Assert.Equal(Ahora + DuracionBloqueo, usuario.BloqueadoHasta);
    }

    [Fact]
    public void Sigue_bloqueado_un_segundo_antes_de_cumplirse_el_plazo()
    {
        var usuario = CrearUsuario();
        FallarVeces(usuario, MaximoIntentos, Ahora);

        var casiDesbloqueado = Ahora + DuracionBloqueo - TimeSpan.FromSeconds(1);

        Assert.True(usuario.EstaBloqueado(casiDesbloqueado));
    }

    [Fact]
    public void Se_desbloquea_cuando_se_cumple_el_plazo()
    {
        var usuario = CrearUsuario();
        FallarVeces(usuario, MaximoIntentos, Ahora);

        Assert.False(usuario.EstaBloqueado(Ahora + DuracionBloqueo));
    }

    [Fact]
    public void Tras_desbloquearse_un_fallo_empieza_la_cuenta_desde_uno()
    {
        var usuario = CrearUsuario();
        FallarVeces(usuario, MaximoIntentos, Ahora);
        var despuesDelBloqueo = Ahora + DuracionBloqueo + TimeSpan.FromSeconds(1);

        FallarVeces(usuario, 1, despuesDelBloqueo);

        Assert.Equal(1, usuario.IntentosFallidos);
        Assert.False(usuario.EstaBloqueado(despuesDelBloqueo));
    }

    [Fact]
    public void Reiniciar_intentos_limpia_el_contador_y_el_bloqueo()
    {
        var usuario = CrearUsuario();
        FallarVeces(usuario, MaximoIntentos, Ahora);

        usuario.ReiniciarIntentos();

        Assert.Equal(0, usuario.IntentosFallidos);
        Assert.Null(usuario.BloqueadoHasta);
        Assert.False(usuario.EstaBloqueado(Ahora));
    }
}
