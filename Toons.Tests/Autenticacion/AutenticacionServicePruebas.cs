using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Toons.Application.Autenticacion;
using Toons.Application.Comun;
using Toons.Domain.Entidades;
using Toons.Infrastructure.Persistencia;
using Toons.Infrastructure.Seguridad;
using Toons.Tests.Apoyo;

namespace Toons.Tests.Autenticacion;

public sealed class AutenticacionServicePruebas : IDisposable
{
    private const string Correo = "usuario@ejemplo.com";
    private const string Contrasena = "clave-de-ejemplo";
    private const string ContrasenaIncorrecta = "otra-clave-distinta";

    private readonly BaseDatosSqlite baseDatos = new();
    private readonly FakeTimeProvider reloj = new(new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero));
    private readonly ToonsDbContext contexto;
    private readonly HasherEspia hasherEspia;
    private readonly AutenticacionService servicio;

    public AutenticacionServicePruebas()
    {
        contexto = baseDatos.CrearContexto();
        var hasher = new PasswordHasherAspNet();
        SembrarUsuario(hasher);
        hasherEspia = new HasherEspia(hasher);
        servicio = new AutenticacionService(
            new LoginSolicitudValidator(),
            new UsuarioRepositorio(contexto),
            hasherEspia,
            new GeneradorTokenFalso(),
            Options.Create(new BloqueoOptions()),
            reloj);
    }

    private void SembrarUsuario(PasswordHasherAspNet hasher)
    {
        var rol = new Rol { Nombre = "lector" };
        contexto.Usuarios.Add(new Usuario
        {
            Nombre = "Usuario de ejemplo",
            Correo = Correo,
            ContrasenaHash = hasher.Cifrar(Contrasena),
            Rol = rol,
            FechaRegistro = reloj.GetUtcNow().UtcDateTime,
        });
        contexto.SaveChanges();
    }

    private Task<Resultado<LoginRespuesta>> Ingresar(string correo, string contrasena)
    {
        return servicio.IniciarSesionAsync(new LoginSolicitud(correo, contrasena), CancellationToken.None);
    }

    private async Task FallarVeces(int veces)
    {
        for (var i = 0; i < veces; i++)
        {
            await Ingresar(Correo, ContrasenaIncorrecta);
        }
    }

    private int IntentosGuardados()
    {
        using var otroContexto = baseDatos.CrearContexto();
        return otroContexto.Usuarios.Single(u => u.Correo == Correo).IntentosFallidos;
    }

    [Fact]
    public async Task Credenciales_correctas_devuelven_token_nombre_y_rol()
    {
        var resultado = await Ingresar(Correo, Contrasena);

        Assert.True(resultado.EsExitoso);
        Assert.Equal(GeneradorTokenFalso.TokenFijo, resultado.Valor.Token);
        Assert.Equal("Usuario de ejemplo", resultado.Valor.Nombre);
        Assert.Equal("lector", resultado.Valor.Rol);
    }

    [Fact]
    public async Task El_correo_se_busca_sin_distinguir_mayusculas_ni_espacios_alrededor()
    {
        var resultado = await Ingresar("  USUARIO@Ejemplo.com ", Contrasena);

        Assert.True(resultado.EsExitoso);
    }

    [Fact]
    public async Task Contrasena_incorrecta_devuelve_credenciales_invalidas_y_suma_un_intento()
    {
        var resultado = await Ingresar(Correo, ContrasenaIncorrecta);

        Assert.False(resultado.EsExitoso);
        Assert.Equal(CodigosError.CredencialesInvalidas, resultado.Error.Codigo);
        Assert.Equal(1, IntentosGuardados());
    }

    [Fact]
    public async Task Usuario_inexistente_devuelve_el_mismo_error_que_contrasena_incorrecta()
    {
        var inexistente = await Ingresar("nadie@ejemplo.com", Contrasena);
        var incorrecta = await Ingresar(Correo, ContrasenaIncorrecta);

        Assert.Equal(incorrecta.Error, inexistente.Error);
    }

    [Fact]
    public async Task Usuario_inexistente_igual_gasta_una_verificacion_de_contrasena()
    {
        await Ingresar("nadie@ejemplo.com", Contrasena);

        Assert.Equal(1, hasherEspia.Simulaciones);
    }

    [Fact]
    public async Task Usuario_existente_no_dispara_la_verificacion_simulada()
    {
        await Ingresar(Correo, ContrasenaIncorrecta);

        Assert.Equal(0, hasherEspia.Simulaciones);
    }

    [Fact]
    public async Task Los_intentos_con_la_cuenta_bloqueada_no_suman_mas_fallos()
    {
        await FallarVeces(5);

        await FallarVeces(3);

        Assert.Equal(5, IntentosGuardados());
    }

    [Fact]
    public async Task Solicitud_invalida_devuelve_el_codigo_de_validacion_con_su_campo()
    {
        var resultado = await Ingresar("sin-arroba", Contrasena);

        Assert.Equal(CodigosError.CorreoInvalido, resultado.Error.Codigo);
        Assert.Equal("correo", resultado.Error.Campo);
    }

    [Fact]
    public async Task Cinco_fallos_bloquean_la_cuenta_y_el_siguiente_intento_devuelve_cuenta_bloqueada()
    {
        await FallarVeces(5);

        var resultado = await Ingresar(Correo, ContrasenaIncorrecta);

        Assert.Equal(CodigosError.CuentaBloqueada, resultado.Error.Codigo);
    }

    [Fact]
    public async Task Cuenta_bloqueada_rechaza_incluso_la_contrasena_correcta()
    {
        await FallarVeces(5);

        var resultado = await Ingresar(Correo, Contrasena);

        Assert.Equal(CodigosError.CuentaBloqueada, resultado.Error.Codigo);
    }

    [Fact]
    public async Task Cuatro_fallos_no_bloquean_y_la_contrasena_correcta_entra()
    {
        await FallarVeces(4);

        var resultado = await Ingresar(Correo, Contrasena);

        Assert.True(resultado.EsExitoso);
    }

    [Fact]
    public async Task Pasados_cinco_minutos_la_cuenta_se_desbloquea()
    {
        await FallarVeces(5);
        reloj.Advance(TimeSpan.FromMinutes(5));

        var resultado = await Ingresar(Correo, Contrasena);

        Assert.True(resultado.EsExitoso);
    }

    [Fact]
    public async Task Un_login_correcto_reinicia_el_contador_guardado()
    {
        await FallarVeces(3);

        await Ingresar(Correo, Contrasena);

        Assert.Equal(0, IntentosGuardados());
    }

    public void Dispose()
    {
        contexto.Dispose();
        baseDatos.Dispose();
    }
}
