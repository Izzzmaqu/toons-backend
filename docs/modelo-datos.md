# Modelo de datos

Propuesta de Persona 3, revisada por Persona 2. Se puede modificar: ver el README para saber cómo.
Son 12 tablas en PostgreSQL con nombres en `snake_case` (paquete `EFCore.NamingConventions`).

```mermaid
erDiagram
    roles ||--o{ usuarios : tiene
    usuarios ||--o{ obras : crea
    idiomas ||--o{ obras : usa
    obras ||--o{ obras_etiquetas : tiene
    etiquetas ||--o{ obras_etiquetas : clasifica
    obras ||--o{ capitulos : contiene
    capitulos ||--o{ paginas : contiene
    usuarios ||--o{ favoritos : marca
    obras ||--o{ favoritos : recibe
    usuarios ||--o{ lecturas : realiza
    capitulos ||--o{ lecturas : recibe
    usuarios ||--o{ comentarios : escribe
    obras ||--o{ comentarios : recibe
    usuarios ||--o{ calificaciones : otorga
    obras ||--o{ calificaciones : recibe

    roles {
        int id PK
        string nombre UK
    }
    usuarios {
        int id PK
        string nombre
        string correo UK
        string contrasena_hash
        int rol_id FK
        int intentos_fallidos
        datetime bloqueado_hasta
        datetime fecha_registro
    }
    idiomas {
        int id PK
        string nombre UK
    }
    obras {
        int id PK
        string titulo
        string descripcion
        string portada
        int creador_id FK
        int idioma_id FK
        datetime fecha_creacion
    }
    etiquetas {
        int id PK
        string nombre UK
    }
    obras_etiquetas {
        int obra_id PK
        int etiqueta_id PK
    }
    capitulos {
        int id PK
        int obra_id FK
        int numero
        string titulo
        datetime fecha_publicacion
    }
    paginas {
        int id PK
        int capitulo_id FK
        int numero
        string url_imagen
    }
    favoritos {
        int usuario_id PK
        int obra_id PK
        datetime fecha_marcado
    }
    lecturas {
        int id PK
        int usuario_id FK
        int capitulo_id FK
        datetime fecha_lectura
    }
    comentarios {
        int id PK
        int usuario_id FK
        int obra_id FK
        string texto
        datetime fecha
    }
    calificaciones {
        int id PK
        int usuario_id FK
        int obra_id FK
        int puntaje
        datetime fecha
    }
```

## Restricciones

| Tabla | Restricción |
|---|---|
| roles, idiomas, etiquetas | `nombre` único |
| usuarios | `correo` único |
| capitulos | único (`obra_id`, `numero`) |
| paginas | único (`capitulo_id`, `numero`) |
| calificaciones | único (`usuario_id`, `obra_id`) y `puntaje` entre 1 y 5 (CHECK) |
| obras_etiquetas | clave primaria compuesta (`obra_id`, `etiqueta_id`) |
| favoritos | clave primaria compuesta (`usuario_id`, `obra_id`) |

## Comportamiento al borrar

- **Cascade** (se borra lo dependiente): filas de `obras_etiquetas` y `favoritos` (tablas puente), `paginas` de un capítulo y `capitulos` de una obra.
- **Restrict** (la base impide borrar si hay dependientes): todas las demás claves foráneas, por ejemplo un usuario con obras, un rol con usuarios, un idioma con obras, un capítulo con lecturas o una obra con comentarios y calificaciones.

## Datos de ejemplo

En Development, `SembradorDatos` llena una base vacía con datos ficticios y deterministas (semilla fija): 2 roles, 5 usuarios, 3 idiomas, 8 etiquetas, 10 obras (`portada-01.svg` a `portada-10.svg`), 40 capítulos, de 3 a 5 páginas por capítulo, 300 lecturas repartidas en los últimos 6 meses, favoritos, comentarios y calificaciones. Las fechas son relativas al momento de sembrar, así los reportes por mes siempre tienen datos recientes.
