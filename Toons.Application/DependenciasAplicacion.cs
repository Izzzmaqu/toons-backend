using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Toons.Application.Autenticacion;
using Toons.Application.Obras;

namespace Toons.Application;

public static class DependenciasAplicacion
{
    public static IServiceCollection AgregarAplicacion(this IServiceCollection servicios)
    {
        servicios.AddScoped<IValidator<LoginSolicitud>, LoginSolicitudValidator>();
        servicios.AddScoped<IAutenticacionService, AutenticacionService>();
        servicios.AddScoped<IObrasService, ObrasService>();
        return servicios;
    }
}
