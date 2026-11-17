# Arquitectura

## Capas y dependencias

Cada flecha significa "depende de". Domain no depende de nadie; Api solo compone.

```mermaid
flowchart TD
    Api --> Application
    Api --> Infrastructure
    Infrastructure --> Application
    Application --> Domain
    Infrastructure --> Domain
```

Las pruebas de `Toons.Tests/Arquitectura` hacen cumplir estas reglas.

## Flujo del login

```mermaid
sequenceDiagram
    participant Navegador
    participant Web as Web (route handler)
    participant Api as Api (controlador)
    participant Servicio as AutenticacionService
    participant Base as Base de datos

    Navegador->>Web: POST /api/sesion (correo y contraseña)
    Web->>Api: POST /api/autenticacion/login
    Api->>Servicio: IniciarSesionAsync
    Servicio->>Servicio: Validar la solicitud
    Servicio->>Base: Buscar usuario por correo
    Base-->>Servicio: Usuario
    Servicio->>Servicio: Comprobar bloqueo y contraseña
    Servicio->>Base: Guardar intentos fallidos o reinicio
    Servicio-->>Api: Resultado (token o código de error)
    Api-->>Web: 200 con token, o 400, 401 o 423 con código
    Web-->>Navegador: Cookie httpOnly, o código de error
```
