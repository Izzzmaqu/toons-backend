using Toons.Application.Autenticacion;
using Toons.Application.Comun;

namespace Toons.Tests.Registro;

public class RegistroSolicitudValidatorPruebas
{
    private const string NombreValido = "Ana Pérez";
    private const string CorreoValido = "ana@ejemplo.com";
    private const string ContrasenaValida = "clave-de-ejemplo";

    private readonly RegistroSolicitudValidator validador = new();

    private string[] CodigosDe(string nombre, string correo, string contrasena)
    {
        var resultado = validador.Validate(new RegistroSolicitud(nombre, correo, contrasena));
        return resultado.Errors.Select(e => e.ErrorCode).ToArray();
    }

    [Fact]
    public void Solicitud_valida_no_tiene_errores()
    {
        Assert.Empty(CodigosDe(NombreValido, CorreoValido, ContrasenaValida));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Nombre_vacio_devuelve_nombre_obligatorio(string nombre)
    {
        Assert.Equal([CodigosError.NombreObligatorio], CodigosDe(nombre, CorreoValido, ContrasenaValida));
    }

    [Fact]
    public void Nombre_de_sesenta_caracteres_es_valido()
    {
        Assert.Empty(CodigosDe(new string('a', 60), CorreoValido, ContrasenaValida));
    }

    [Fact]
    public void Nombre_de_sesenta_y_un_caracteres_devuelve_nombre_largo()
    {
        Assert.Equal([CodigosError.NombreLargo], CodigosDe(new string('a', 61), CorreoValido, ContrasenaValida));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Correo_vacio_devuelve_correo_obligatorio(string correo)
    {
        Assert.Equal([CodigosError.CorreoObligatorio], CodigosDe(NombreValido, correo, ContrasenaValida));
    }

    [Theory]
    [InlineData("sin-arroba")]
    [InlineData("@ejemplo.com")]
    [InlineData("ana@")]
    [InlineData("a@@b.com")]
    public void Correo_con_formato_malo_devuelve_correo_invalido(string correo)
    {
        Assert.Equal([CodigosError.CorreoInvalido], CodigosDe(NombreValido, correo, ContrasenaValida));
    }

    [Theory]
    [InlineData("")]
    [InlineData("        ")]
    public void Contrasena_vacia_devuelve_contrasena_obligatoria(string contrasena)
    {
        Assert.Equal([CodigosError.ContrasenaObligatoria], CodigosDe(NombreValido, CorreoValido, contrasena));
    }

    [Fact]
    public void Contrasena_de_siete_caracteres_devuelve_contrasena_corta()
    {
        Assert.Equal([CodigosError.ContrasenaCorta], CodigosDe(NombreValido, CorreoValido, "1234567"));
    }

    [Fact]
    public void Contrasena_de_ocho_caracteres_es_valida()
    {
        Assert.Empty(CodigosDe(NombreValido, CorreoValido, "12345678"));
    }

    [Fact]
    public void Cada_error_se_asocia_a_su_campo()
    {
        var resultado = validador.Validate(new RegistroSolicitud("", "", ""));

        var errores = resultado.Errors.Select(e => (e.PropertyName, e.ErrorCode)).ToArray();

        Assert.Contains((nameof(RegistroSolicitud.Nombre), CodigosError.NombreObligatorio), errores);
        Assert.Contains((nameof(RegistroSolicitud.Correo), CodigosError.CorreoObligatorio), errores);
        Assert.Contains((nameof(RegistroSolicitud.Contrasena), CodigosError.ContrasenaObligatoria), errores);
    }

    [Fact]
    public void Lo_aceptado_siempre_cumple_la_definicion_independiente()
    {
        var azar = new Random(20260102);
        const string alfabeto = "ab1@. \t";

        for (var i = 0; i < 2000; i++)
        {
            var nombre = TextoAleatorio(azar, alfabeto, azar.Next(0, 70));
            var correo = TextoAleatorio(azar, alfabeto, azar.Next(0, 12));
            var contrasena = TextoAleatorio(azar, alfabeto, azar.Next(0, 12));

            var codigos = CodigosDe(nombre, correo, contrasena);

            if (!codigos.Contains(CodigosError.NombreObligatorio) && !codigos.Contains(CodigosError.NombreLargo))
            {
                Assert.True(EsNombreAceptable(nombre), $"Se aceptó el nombre '{nombre}'");
            }

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

    private static bool EsNombreAceptable(string nombre)
    {
        return !string.IsNullOrWhiteSpace(nombre) && nombre.Length <= 60;
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
