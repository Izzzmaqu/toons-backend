using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Toons.Application.Autenticacion;
using Toons.Application.Comun;
using Toons.Domain.Entidades;
using Toons.Infrastructure.Persistencia;
using Toons.Infrastructure.Seguridad;
using Toons.Tests.Apoyo;

namespace Toons.Tests.Registro;

public sealed class RegistroServicePruebas : IDisposable
{
    private const string Contrasena = "clave-de-ejemplo";

    private readonly BaseDatosSqlite baseDatos = new();
    private readonly FakeTimeProvider reloj = new(new DateTimeOffset(2026, 3, 10, 9, 30, 0, TimeSpan.Zero));
    private readonly PasswordHasherAspNet hasher = new();
    private readonly ToonsDbContext contexto;
    private readonly RegistroService servicio;

    public RegistroServicePruebas()
    {
        contexto = baseDatos.CrearContexto();
        contexto.Roles.AddRange(
            new Rol { Nombre = NombresDeRol.Administrador },
            new Rol { Nombre = NombresDeRol.Lector });
        contexto.SaveChanges();
        servicio = new RegistroService(
            new RegistroSolicitudValidator(),
            new UsuarioRepositorio(contexto),
            new RolRepositorio(contexto),
            hasher,
            new GeneradorTokenFalso(),
            reloj);
    }

    private Task<Resultado<LoginRespuesta>> Registrar(string nombre, string correo, string contrasena = Contrasena)
    {
        return servicio.RegistrarAsync(new RegistroSolicitud(nombre, correo, contrasena), CancellationToken.None);
    }

    private Usuario UsuarioGuardado(string correo)
    {
        using var otroContexto = baseDatos.CrearContexto();
        return otroContexto.Usuarios.Include(u => u.Rol).Single(u => u.Correo == correo);
    }

    private int CantidadDeUsuarios()
    {
        using var otroContexto = baseDatos.CrearContexto();
        return otroContexto.Usuarios.Count();
    }

    [Fact]
    public async Task Registro_exitoso_devuelve_token_nombre_y_rol_lector()
    {
        var resultado = await Registrar("Ana Pérez", "ana@ejemplo.com");

        Assert.True(resultado.EsExitoso);
        Assert.Equal(GeneradorTokenFalso.TokenFijo, resultado.Valor.Token);
        Assert.Equal("Ana Pérez", resultado.Valor.Nombre);
        Assert.Equal(NombresDeRol.Lector, resultado.Valor.Rol);
    }

    [Fact]
    public async Task Guarda_el_correo_en_minusculas_y_el_nombre_sin_espacios_alrededor()
    {
        await Registrar("  Ana Pérez  ", "  ANA@Ejemplo.COM ");

        var usuario = UsuarioGuardado("ana@ejemplo.com");
        Assert.Equal("Ana Pérez", usuario.Nombre);
    }

    [Fact]
    public async Task Guarda_el_hash_de_la_contrasena_y_no_la_contrasena()
    {
        await Registrar("Ana Pérez", "ana@ejemplo.com");

        var usuario = UsuarioGuardado("ana@ejemplo.com");
        Assert.NotEqual(Contrasena, usuario.ContrasenaHash);
        Assert.True(hasher.Verificar(Contrasena, usuario.ContrasenaHash));
    }

    [Fact]
    public async Task El_rol_por_defecto_es_lector_aunque_exista_administrador()
    {
        await Registrar("Ana Pérez", "ana@ejemplo.com");

        Assert.Equal(NombresDeRol.Lector, UsuarioGuardado("ana@ejemplo.com").Rol.Nombre);
    }

    [Fact]
    public async Task El_usuario_nuevo_empieza_sin_intentos_fallidos_y_con_la_fecha_de_registro_actual()
    {
        await Registrar("Ana Pérez", "ana@ejemplo.com");

        var usuario = UsuarioGuardado("ana@ejemplo.com");
        Assert.Equal(0, usuario.IntentosFallidos);
        Assert.Null(usuario.BloqueadoHasta);
        Assert.Equal(reloj.GetUtcNow().UtcDateTime, usuario.FechaRegistro);
    }

    [Fact]
    public async Task Correo_repetido_devuelve_correo_en_uso_y_no_crea_otro_usuario()
    {
        await Registrar("Ana Pérez", "ana@ejemplo.com");

        var resultado = await Registrar("Otra Ana", "ana@ejemplo.com");

        Assert.False(resultado.EsExitoso);
        Assert.Equal(CodigosError.CorreoEnUso, resultado.Error.Codigo);
        Assert.Equal("correo", resultado.Error.Campo);
        Assert.Equal(1, CantidadDeUsuarios());
    }

    [Fact]
    public async Task Correo_repetido_se_detecta_sin_distinguir_mayusculas()
    {
        await Registrar("Ana Pérez", "ana@ejemplo.com");

        var resultado = await Registrar("Otra Ana", "ANA@Ejemplo.COM");

        Assert.Equal(CodigosError.CorreoEnUso, resultado.Error.Codigo);
        Assert.Equal(1, CantidadDeUsuarios());
    }

    [Fact]
    public async Task Dos_correos_distintos_se_registran_los_dos()
    {
        var primero = await Registrar("Ana Pérez", "ana@ejemplo.com");
        var segundo = await Registrar("Beto Ruiz", "beto@ejemplo.com");

        Assert.True(primero.EsExitoso);
        Assert.True(segundo.EsExitoso);
        Assert.Equal(2, CantidadDeUsuarios());
    }

    [Fact]
    public async Task Solicitud_invalida_devuelve_el_codigo_con_su_campo_y_no_guarda_nada()
    {
        var resultado = await Registrar("", "ana@ejemplo.com");

        Assert.Equal(CodigosError.NombreObligatorio, resultado.Error.Codigo);
        Assert.Equal("nombre", resultado.Error.Campo);
        Assert.Equal(0, CantidadDeUsuarios());
    }

    [Fact]
    public async Task Sin_el_rol_lector_en_la_base_falla_con_excepcion_y_no_guarda_nada()
    {
        contexto.Roles.Remove(contexto.Roles.Single(r => r.Nombre == NombresDeRol.Lector));
        contexto.SaveChanges();

        await Assert.ThrowsAsync<InvalidOperationException>(() => Registrar("Ana Pérez", "ana@ejemplo.com"));

        Assert.Equal(0, CantidadDeUsuarios());
    }

    [Fact]
    public async Task Quien_se_registra_puede_iniciar_sesion_con_las_mismas_credenciales()
    {
        await Registrar("Ana Pérez", "ana@ejemplo.com");
        var autenticacion = new AutenticacionService(
            new LoginSolicitudValidator(),
            new UsuarioRepositorio(baseDatos.CrearContexto()),
            hasher,
            new GeneradorTokenFalso(),
            Options.Create(new BloqueoOptions()),
            reloj);

        var resultado = await autenticacion.IniciarSesionAsync(
            new LoginSolicitud("Ana@Ejemplo.com", Contrasena), CancellationToken.None);

        Assert.True(resultado.EsExitoso);
        Assert.Equal(NombresDeRol.Lector, resultado.Valor.Rol);
    }

    public void Dispose()
    {
        contexto.Dispose();
        baseDatos.Dispose();
    }
}
