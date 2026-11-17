using Toons.Api.Configuracion;
using Toons.Application;
using Toons.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var cadenaConexion = builder.Configuration.GetConnectionString("Toons")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'Toons'.");
var origenFrontend = builder.Configuration["OrigenFrontend"]
    ?? throw new InvalidOperationException("Falta la configuración 'OrigenFrontend'.");

builder.Services.AgregarOpcionesTipadas(builder.Configuration);
builder.Services.AgregarAplicacion();
builder.Services.AgregarInfraestructura(cadenaConexion);
builder.Services.AgregarAutenticacionJwt();
builder.Services.AgregarCorsDelFrontend(origenFrontend);
builder.Services.AgregarManejoDeErrores();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.PrepararBaseDeDatosDeDesarrolloAsync();
}

app.UseExceptionHandler(_ => { });
app.UsarCorsDelFrontend();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
