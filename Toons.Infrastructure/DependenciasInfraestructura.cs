using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Toons.Application.Autenticacion;
using Toons.Application.Obras;
using Toons.Infrastructure.Persistencia;
using Toons.Infrastructure.Seguridad;

namespace Toons.Infrastructure;

public static class DependenciasInfraestructura
{
    public static IServiceCollection AgregarInfraestructura(this IServiceCollection servicios, string cadenaConexion)
    {
        servicios.AddDbContext<ToonsDbContext>(opciones => opciones
            .UseNpgsql(cadenaConexion)
            .UseSnakeCaseNamingConvention());
        servicios.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
        servicios.AddScoped<IObraRepositorio, ObraRepositorio>();
        servicios.AddScoped<SembradorDatos>();
        servicios.AddSingleton<IPasswordHasher, PasswordHasherAspNet>();
        servicios.AddSingleton<IGeneradorToken, GeneradorTokenJwt>();
        servicios.AddSingleton(TimeProvider.System);
        return servicios;
    }
}
