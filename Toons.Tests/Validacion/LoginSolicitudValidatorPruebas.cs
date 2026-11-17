using Toons.Application.Autenticacion;
using Toons.Application.Comun;

namespace Toons.Tests.Validacion;

public class LoginSolicitudValidatorPruebas
{
    private const string ContrasenaValida = "clave-de-ejemplo";
    private const string CorreoValido = "persona@ejemplo.com";

    private readonly LoginSolicitudValidator validador = new();

    private string[] CodigosDe(string correo, string contrasena)
    {
        var resultado = validador.Validate(new LoginSolicitud(correo, contrasena));
        return resultado.Errors.Select(e => e.ErrorCode).ToArray();
    }

    [Fact]
    public void Solicitud_valida_no_tiene_errores()
    {
        Assert.Empty(CodigosDe(CorreoValido, ContrasenaValida));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Correo_vacio_devuelve_correo_obligatorio(string correo)
    {
        Assert.Equal([CodigosError.CorreoObligatorio], CodigosDe(correo, ContrasenaValida));
    }

    [Theory]
    [InlineData("sin-arroba")]
    [InlineData("@ejemplo.com")]
    [InlineData("persona@")]
    [InlineData("a@@b.com")]
    public void Correo_con_formato_malo_devuelve_correo_invalido(string correo)
    {
        Assert.Equal([CodigosError.CorreoInvalido], CodigosDe(correo, ContrasenaValida));
    }

    [Theory]
    [InlineData("")]
    [InlineData("        ")]
    public void Contrasena_vacia_devuelve_contrasena_obligatoria(string contrasena)
    {
        Assert.Equal([CodigosError.ContrasenaObligatoria], CodigosDe(CorreoValido, contrasena));
    }

    [Fact]
    public void Contrasena_de_siete_caracteres_devuelve_contrasena_corta()
    {
        Assert.Equal([CodigosError.ContrasenaCorta], CodigosDe(CorreoValido, "1234567"));
    }

    [Fact]
    public void Contrasena_de_ocho_caracteres_es_valida()
    {
        Assert.Empty(CodigosDe(CorreoValido, "12345678"));
    }

    [Fact]
    public void Correo_y_contrasena_malos_devuelven_ambos_errores_cada_uno_en_su_campo()
    {
        var resultado = validador.Validate(new LoginSolicitud("", ""));

        var errores = resultado.Errors.Select(e => (e.PropertyName, e.ErrorCode)).ToArray();

        Assert.Contains((nameof(LoginSolicitud.Correo), CodigosError.CorreoObligatorio), errores);
        Assert.Contains((nameof(LoginSolicitud.Contrasena), CodigosError.ContrasenaObligatoria), errores);
    }

    [Fact]
    public void Lo_aceptado_siempre_cumple_la_definicion_independiente()
    {
        var azar = new Random(20260101);
        const string alfabeto = "ab1@. \t";

        for (var i = 0; i < 2000; i++)
        {
            var correo = TextoAleatorio(azar, alfabeto, azar.Next(0, 12));
            var contrasena = TextoAleatorio(azar, alfabeto, azar.Next(0, 12));

            var codigos = CodigosDe(correo, contrasena);

            if (!codigos.Contains(CodigosError.CorreoObligatorio) && !codigos.Contains(CodigosError.CorreoInvalido))
            {
                Assert.True(EsCorreoAceptable(correo), $"Se aceptó el correo '{correo}'");
            }

            if (!codigos.Contains(CodigosError.ContrasenaObligatoria) && !codigos.Contains(CodigosError.ContrasenaCorta))
            {
                Assert.True(EsContrasenaAceptable(contrasena), $"Se aceptó la contraseña '{contrasena}'");
            }
        }
    }

    private static string TextoAleatorio(Random azar, string alfabeto, int largo)
    {
        return string.Concat(Enumerable.Range(0, largo).Select(_ => alfabeto[azar.Next(alfabeto.Length)]));
    }

    private static bool EsCorreoAceptable(string correo)
    {
        var partes = correo.Split('@');
        return partes.Length == 2
            && partes[0].Length > 0
            && partes[1].Length > 0
            && !string.IsNullOrWhiteSpace(correo);
    }

    private static bool EsContrasenaAceptable(string contrasena)
    {
        return contrasena.Length >= 8 && !string.IsNullOrWhiteSpace(contrasena);
    }
}
