# Toons — API (backend)

API en ASP.NET Core (.NET 10) con EF Core y PostgreSQL para la plataforma de lectura de cómics "Toons".
El frontend vive en otro repositorio: `toons-web`.

## Requisitos

| Herramienta | Versión | Cómo comprobarla |
|---|---|---|
| SDK de .NET | 10.0.100 o superior de la serie 10 (`global.json` lo exige) | `dotnet --version` |
| Docker Desktop | cualquiera reciente (o un PostgreSQL 16 ya instalado) | `docker --version` |
| Git | cualquiera reciente | `git --version` |

Descarga el SDK de .NET 10 desde https://dotnet.microsoft.com/download/dotnet/10.0. Si prefieres no tocar tu instalación, puedes bajarlo a una carpeta aparte con el script oficial:

```powershell
Invoke-WebRequest https://dot.net/v1/dotnet-install.ps1 -OutFile dotnet-install.ps1
.\dotnet-install.ps1 -Channel 10.0 -InstallDir C:\sdk-dotnet10
$env:PATH = "C:\sdk-dotnet10;$env:PATH"
```

(El cambio de `PATH` vale solo para esa ventana de PowerShell.)

## Paso a paso desde cero (PowerShell)

1. Consigue el código y entra a la carpeta:

   ```powershell
   git clone <URL-del-repositorio-toons-backend>
   cd toons-backend
   dotnet --version
   ```

   `dotnet --version` debe mostrar 10.0.x. Si muestra otra versión o un error de "SDK not found", instala el SDK 10 (ver Requisitos).

2. Levanta PostgreSQL con Docker (puerto 5433 del equipo, base `toons`; usuario y contraseña ficticios de solo desarrollo):

   ```powershell
   docker compose up -d
   docker compose ps
   ```

   `docker compose ps` debe mostrar `toons-postgres` en estado `running`.

   **Con un PostgreSQL ya instalado** (sin Docker): crea una base `toons` y un usuario con permisos sobre ella, y define la cadena de conexión antes de ejecutar la API:

   ```powershell
   $env:ConnectionStrings__Toons = "Host=localhost;Port=5432;Database=toons;Username=tu_usuario;Password=tu_clave"
   ```

   (También puedes editar `ConnectionStrings:Toons` en `Toons.Api/appsettings.Development.json`. `.env.example` muestra el formato.)

3. Ejecuta la API (queda en http://localhost:5080). En Development crea las tablas y los datos de ejemplo al arrancar:

   ```powershell
   dotnet run --project Toons.Api
   ```

   Deja esa ventana abierta; con `Ctrl+C` se detiene.

4. En otra ventana, ejecuta las pruebas (no necesitan PostgreSQL: usan SQLite en memoria):

   ```powershell
   dotnet test Toons.slnx
   ```

5. Para el sitio web, sigue el README de `toons-web`.

Para apagar PostgreSQL: `docker compose down` (conserva los datos) o `docker compose down -v` (los borra).

## Si `Toons.slnx` no abre

`Toons.slnx` abre con Visual Studio 2022 17.13 o superior, Visual Studio 2026, y con VS Code + la extensión C# Dev Kit. Si tu editor no lo abre, genera una solución clásica con:

```powershell
dotnet sln Toons.slnx migrate
```

Eso crea `Toons.sln` (no la subas al repositorio).

## Si algo falla

- **El puerto 5433 o 5080 está ocupado:** apaga lo que lo usa o cambia el puerto en `docker-compose.yml` (y en la cadena de conexión) o en `Toons.Api/Properties/launchSettings.json`.
- **La API no arranca por no conectar a la base:** comprueba `docker compose ps` y que la cadena de conexión coincida.
- **La API dice que falta configuración (`Jwt`):** fuera de Development la clave JWT es obligatoria; ejecuta con `dotnet run --project Toons.Api` (modo Development por defecto) o define `Jwt__Clave` con un valor de al menos 32 caracteres.

## Usuarios de ejemplo (ficticios, solo desarrollo)

| Correo | Contraseña | Rol |
|---|---|---|
| admin@ejemplo.com | admin-ejemplo-123 | administrador |
| usuario@ejemplo.com | usuario-ejemplo-123 | lector |
| lucia@ejemplo.com, mateo@ejemplo.com, sofia@ejemplo.com | lector-ejemplo-123 | lector |

## Estructura

```
Toons.slnx
Toons.Domain          Entidades y reglas del dominio. No depende de nada.
Toons.Application     Casos de uso (servicios), DTOs, validadores, interfaces y códigos de error. Solo depende de Domain.
Toons.Infrastructure  EF Core (contexto, configuraciones, migraciones), repositorios, hasher y generador de JWT. Depende de Application y Domain.
Toons.Api             Controladores delgados y composición de dependencias. Compone todas las capas.
Toons.Tests           Pruebas (xUnit, SQLite en memoria y pruebas de arquitectura con NetArchTest).
docs/modelo-datos.md  Diagrama de las 12 tablas.
docs/arquitectura.md  Capas y flujo del login.
```

La regla de dependencias la hacen cumplir las pruebas de `Toons.Tests/Arquitectura`.

## Errores por código

El backend nunca devuelve mensajes de texto: solo códigos con el formato `modulo.caso` (ver `Toons.Application/Comun/CodigosError.cs`). La API responde `{ "codigo": "...", "campo": "..." }` con HTTP 400 (validación), 401 (credenciales inválidas), 409 (correo ya registrado), 423 (cuenta bloqueada) o 500 (error interno). El frontend traduce cada código a un mensaje en español.

## Registro

`POST /api/autenticacion/registro` con `{ "nombre": "...", "correo": "...", "contrasena": "..." }` crea la cuenta y responde igual que el login (`{ token, nombre, rol }`), así el frontend puede entrar directo.

- **Nombre:** obligatorio, máximo 60 caracteres (`registro.nombre_obligatorio`, `registro.nombre_largo`).
- **Correo:** obligatorio y con formato válido (`auth.correo_obligatorio`, `auth.correo_invalido`). Se guarda en minúsculas; "ANA@x.com" y "ana@x.com" son el mismo correo.
- **Contraseña:** obligatoria, mínimo 8 caracteres (`auth.contrasena_obligatoria`, `auth.contrasena_corta`).
- **Correo repetido:** `registro.correo_en_uso` con HTTP 409. La detección final la hace el índice único de la base, así dos registros simultáneos con el mismo correo no pueden crear dos cuentas. Igual que en el bloqueo del login, esto revela que el correo ya existe; se acepta porque la persona necesita saber por qué no puede registrarse.
- **Rol:** siempre `lector`. La solicitud no puede pedir otro rol; el rol `administrador` solo se asigna directamente en la base.

## Decisiones de seguridad del login

- **Cuenta bloqueada (423):** tras 5 intentos fallidos la cuenta se bloquea 5 minutos (`Bloqueo` en `appsettings.json`). Mientras esté bloqueada se responde `auth.cuenta_bloqueada` aunque la contraseña sea correcta. Esto **revela que el correo existe**; se aceptó a propósito porque la persona legítima necesita saber por qué no puede entrar. Cualquier otro fallo devuelve el mismo `auth.credenciales_invalidas`, exista o no el correo.
- **Tiempo de respuesta:** para un correo inexistente se verifica la contraseña contra un hash ficticio, así el tiempo no delata si el correo existe.
- **Intentos en paralelo (limitación conocida):** el contador se lee, se suma y se guarda en el mismo registro. Con muchas peticiones simultáneas contra la misma cuenta pueden perderse incrementos y permitirse algo más de 5 intentos. Un incremento atómico exigiría traducir las reglas del dominio a SQL, lo que se descartó por complejidad.
- Los valores de `appsettings.Development.json` (`SOLO-DESARROLLO-CAMBIAR`) son ficticios y solo sirven en desarrollo.
- El login busca el correo en minúsculas: los usuarios nuevos deben guardarse con el correo en minúsculas.

## Para Persona 2: el modelo de datos

El modelo (`docs/modelo-datos.md`) es una **propuesta de Persona 3, revisada por Persona 2, y se puede modificar**.

**Cómo añadir o cambiar una entidad:**

1. Crea o edita la clase en `Toons.Domain/Entidades/` (propiedades simples y de navegación; sin lógica de base de datos).
2. Añade su `DbSet` en `Toons.Infrastructure/Persistencia/ToonsDbContext.cs`.
3. Copia una configuración de `Toons.Infrastructure/Persistencia/Configuraciones/` (por ejemplo `CapituloConfiguracion.cs`) y adáptala: nombre de tabla, longitudes, únicos, claves foráneas y comportamiento al borrar (`Restrict` salvo tablas puente, páginas y capítulos). Se aplican solas.
4. Crea la migración (la primera vez, `dotnet tool restore`):

   ```powershell
   dotnet tool restore
   dotnet tool run dotnet-ef migrations add NombreDeLaMigracion --project Toons.Infrastructure --startup-project Toons.Api
   ```

5. Arranca la API: en Development aplica la migración sola. Añade una prueba en `Toons.Tests/Modelo/`.

**Reiniciar la base de desarrollo** (borra todo y vuelve a sembrar los datos de ejemplo):

```powershell
docker compose down -v
docker compose up -d
dotnet run --project Toons.Api
```

## Para Persona 3: tablas y reportes

Las tablas están descritas en `docs/modelo-datos.md`. Para consultarlas con `psql` dentro del contenedor de Docker:

```powershell
docker compose exec postgres psql -U toons_dev -d toons
```

Dentro de `psql`: `\dt` lista las tablas, `\d obras` describe una tabla y `\q` sale. Ejemplos útiles para reportes:

```sql
SELECT date_trunc('month', fecha_lectura) AS mes, count(*) AS lecturas
FROM lecturas GROUP BY mes ORDER BY mes;

SELECT u.nombre AS creador, count(*) AS lecturas
FROM lecturas l
JOIN capitulos c ON c.id = l.capitulo_id
JOIN obras o ON o.id = c.obra_id
JOIN usuarios u ON u.id = o.creador_id
GROUP BY u.nombre ORDER BY lecturas DESC;
```

**Cómo añadir un reporte a la API** (sigue el patrón de `Obras`):

1. En `Toons.Application/Reportes/` crea el DTO del resultado, la interfaz `IReporteRepositorio` y un `ReportesService` con su interfaz.
2. En `Toons.Infrastructure/Persistencia/` implementa `ReporteRepositorio` con consultas LINQ (`GroupBy`, `Count`, `Sum`).
3. Regístralos en `DependenciasAplicacion.cs` y `DependenciasInfraestructura.cs`.
4. Crea un `ReportesController` delgado en `Toons.Api/Controllers/` que solo llame al servicio.
5. Escribe pruebas en `Toons.Tests/` usando `BaseSembrada` (base SQLite con los datos de ejemplo).

## Obras

`IObraRepositorio` se implementa con EF Core (`ObraRepositorio`). **Recientes** ordena por la fecha de publicación del último capítulo, **Tendencias** por la cantidad de lecturas de los últimos 30 días y la búsqueda ignora mayúsculas y minúsculas.
