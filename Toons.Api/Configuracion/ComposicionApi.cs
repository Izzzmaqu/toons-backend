using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Toons.Api.Errores;
using Toons.Application.Autenticacion;
using Toons.Application.Comun;
using Toons.Infrastructure.Persistencia;
using Toons.Infrastructure.Seguridad;

namespace Toons.Api.Configuracion;

public static class ComposicionApi
{
    private const string PoliticaCorsFrontend = "Frontend";

    public static IServiceCollection AgregarOpcionesTipadas(this IServiceCollection servicios, IConfiguration configuracion)
    {
        servicios.AddOptions<BloqueoOptions>()
            .Bind(configuracion.GetSection(BloqueoOptions.Seccion))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        servicios.AddOptions<JwtOptions>()
            .Bind(configuracion.GetSection(JwtOptions.Seccion))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        return servicios;
    }

    public static IServiceCollection AgregarAutenticacionJwt(this IServiceCollection servicios)
    {
        servicios.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        servicios.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, jwt) =>
            {
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwt.Value.Emisor,
                    ValidAudience = jwt.Value.Audiencia,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Value.Clave)),
                    NameClaimType = GeneradorTokenJwt.TipoClaimNombre,
                    RoleClaimType = GeneradorTokenJwt.TipoClaimRol,
                };
            });
        servicios.AddAuthorization();
        return servicios;
    }

    public static IServiceCollection AgregarCorsDelFrontend(this IServiceCollection servicios, string origenFrontend)
    {
        servicios.AddCors(opciones => opciones.AddPolicy(
            PoliticaCorsFrontend,
            politica => politica.WithOrigins(origenFrontend).AllowAnyHeader().AllowAnyMethod()));
        return servicios;
    }

    public static IServiceCollection AgregarManejoDeErrores(this IServiceCollection servicios)
    {
        servicios.AddExceptionHandler<ManejadorExcepciones>();
        servicios.AddControllers().ConfigureApiBehaviorOptions(opciones =>
            opciones.InvalidModelStateResponseFactory = _ =>
                RespuestaError.Desde(new ErrorAplicacion(CodigosError.SolicitudInvalida)));
        return servicios;
    }

    public static IApplicationBuilder UsarCorsDelFrontend(this IApplicationBuilder aplicacion)
    {
        return aplicacion.UseCors(PoliticaCorsFrontend);
    }

    public static async Task PrepararBaseDeDatosDeDesarrolloAsync(this WebApplication aplicacion)
    {
        await using var alcance = aplicacion.Services.CreateAsyncScope();
        var contexto = alcance.ServiceProvider.GetRequiredService<ToonsDbContext>();
        await contexto.Database.EnsureCreatedAsync();
        await alcance.ServiceProvider.GetRequiredService<SembradorDatos>().SembrarAsync(CancellationToken.None);
    }
}
