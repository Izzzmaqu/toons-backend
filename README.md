# Toons — API (backend)

API en ASP.NET Core (.NET 10) con EF Core y PostgreSQL para la plataforma de lectura de cómics "Toons".
El frontend vive en otro repositorio: `toons-web`.

## Requisitos

- **SDK de .NET 10** (`global.json` fija la versión 10.0.100 y permite subir de versión de características). Compruébalo con `dotnet --version`.
- Docker (para PostgreSQL) o un PostgreSQL ya instalado.
- `Toons.slnx` abre con Visual Studio 2022 17.13 o superior (o VS 2026) y con VS Code + C# Dev Kit. Si a alguien no le abre, `dotnet sln Toons.slnx migrate` genera un `.sln` clásico.

## Paso a paso

1. Levanta PostgreSQL (puerto 5433 del equipo, base `toons`):

   ```
   docker compose up -d
   ```

   Si ya tienes PostgreSQL instalado, crea una base `toons` y cambia la cadena de conexión `ConnectionStrings:Toons` en `Toons.Api/appsettings.Development.json` (o con la variable de entorno `ConnectionStrings__Toons`, ver `.env.example`).

2. Ejecuta la API (queda en http://localhost:5080). En Development crea las tablas y los datos de ejemplo al arrancar:

   ```
   dotnet run --project Toons.Api
   ```

3. Ejecuta las pruebas:

   ```
   dotnet test Toons.slnx
   ```

## Usuarios de ejemplo (ficticios, solo desarrollo)

| Correo | Contraseña | Rol |
|---|---|---|
| admin@ejemplo.com | admin-ejemplo-123 | administrador |
| usuario@ejemplo.com | usuario-ejemplo-123 | lector |

## Estructura

```
Toons.slnx
Toons.Domain          Entidades y reglas del dominio. No depende de nada.
Toons.Application     Casos de uso (servicios), DTOs, validadores, interfaces y códigos de error. Solo depende de Domain.
Toons.Infrastructure  EF Core, repositorios, hasher de contraseñas y generador de JWT. Depende de Application y Domain.
Toons.Api             Controladores delgados y composición de dependencias. Compone todas las capas.
Toons.Tests           Pruebas (xUnit, SQLite en memoria y pruebas de arquitectura con NetArchTest).
```

La regla de dependencias la hacen cumplir las pruebas de `Toons.Tests/Arquitectura`.

## Errores por código

El backend nunca devuelve mensajes de texto: solo códigos con el formato `modulo.caso` (ver `Toons.Application/Comun/CodigosError.cs`). La API responde `{ "codigo": "...", "campo": "..." }` con HTTP 400 (validación), 401 (credenciales inválidas), 423 (cuenta bloqueada) o 500 (error interno). El frontend traduce cada código a un mensaje en español.

## Decisiones de seguridad del login

- **Cuenta bloqueada (423):** tras 5 intentos fallidos la cuenta se bloquea 5 minutos (`Bloqueo` en `appsettings.json`). Mientras esté bloqueada se responde `auth.cuenta_bloqueada` aunque la contraseña sea correcta. Esto **revela que el correo existe**; se aceptó a propósito porque la persona legítima necesita saber por qué no puede entrar. Cualquier otro fallo devuelve el mismo `auth.credenciales_invalidas`, exista o no el correo.
- **Tiempo de respuesta:** para un correo inexistente se verifica la contraseña contra un hash ficticio, así el tiempo no delata si el correo existe.
- **Intentos en paralelo (limitación conocida):** el contador se lee, se suma y se guarda en el mismo registro. Con muchas peticiones simultáneas contra la misma cuenta pueden perderse incrementos y permitirse algo más de 5 intentos. Un incremento atómico exigiría traducir las reglas del dominio a SQL, lo que se descartó por complejidad.
- Los valores de `appsettings.Development.json` (`SOLO-DESARROLLO-CAMBIAR`) son ficticios y solo sirven en desarrollo. Fuera de Development la API no arranca si falta la clave JWT.
