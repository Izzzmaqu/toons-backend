using NetArchTest.Rules;
using Toons.Application.Comun;
using Toons.Domain.Entidades;
using Toons.Infrastructure.Persistencia;

namespace Toons.Tests.Arquitectura;

public class ReglasDeDependenciaPruebas
{
    private const string NombreApplication = "Toons.Application";
    private const string NombreInfrastructure = "Toons.Infrastructure";
    private const string NombreApi = "Toons.Api";

    private static void AfirmarSinDependencias(Type tipoDelEnsamblado, params string[] prohibidas)
    {
        var resultado = Types.InAssembly(tipoDelEnsamblado.Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(prohibidas)
            .GetResult();

        var infractores = resultado.FailingTypeNames ?? [];
        Assert.True(resultado.IsSuccessful, $"Dependencias prohibidas en: {string.Join(", ", infractores)}");
    }

    [Fact]
    public void Domain_no_depende_de_ninguna_otra_capa_ni_de_librerias_externas()
    {
        AfirmarSinDependencias(
            typeof(Usuario),
            NombreApplication,
            NombreInfrastructure,
            NombreApi,
            "Microsoft.EntityFrameworkCore",
            "Microsoft.AspNetCore",
            "FluentValidation");
    }

    [Fact]
    public void Application_solo_depende_de_Domain()
    {
        AfirmarSinDependencias(
            typeof(CodigosError),
            NombreInfrastructure,
            NombreApi,
            "Microsoft.EntityFrameworkCore",
            "Microsoft.AspNetCore");
    }

    [Fact]
    public void Infrastructure_no_depende_de_Api()
    {
        AfirmarSinDependencias(typeof(ToonsDbContext), NombreApi);
    }
}
