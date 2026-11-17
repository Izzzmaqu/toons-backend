using Microsoft.EntityFrameworkCore;
using Toons.Tests.Apoyo;

namespace Toons.Tests.Modelo;

public sealed class EstructuraDelModeloPruebas : IDisposable
{
    private readonly BaseDatosSqlite baseDatos = new();

    private List<string> Consultar(string sql)
    {
        using var contexto = baseDatos.CrearContexto();
        using var comando = contexto.Database.GetDbConnection().CreateCommand();
        comando.CommandText = sql;
        using var lector = comando.ExecuteReader();
        var filas = new List<string>();
        while (lector.Read())
        {
            filas.Add(lector.GetString(0));
        }

        return filas;
    }

    private List<string> ColumnasDe(string tabla)
    {
        return Consultar($"SELECT name FROM pragma_table_info('{tabla}')");
    }

    [Fact]
    public void Existen_las_doce_tablas_con_nombres_en_snake_case()
    {
        var tablas = Consultar("SELECT name FROM sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite_%'");

        var esperadas = new[]
        {
            "roles", "usuarios", "idiomas", "obras", "etiquetas", "obras_etiquetas",
            "capitulos", "paginas", "favoritos", "lecturas", "comentarios", "calificaciones",
        };
        Assert.Equal(esperadas.Order(), tablas.Order());
    }

    [Theory]
    [InlineData("usuarios", new[] { "id", "nombre", "correo", "contrasena_hash", "rol_id", "intentos_fallidos", "bloqueado_hasta", "fecha_registro" })]
    [InlineData("obras", new[] { "id", "titulo", "descripcion", "portada", "creador_id", "idioma_id", "fecha_creacion" })]
    [InlineData("capitulos", new[] { "id", "obra_id", "numero", "titulo", "fecha_publicacion" })]
    [InlineData("paginas", new[] { "id", "capitulo_id", "numero", "url_imagen" })]
    [InlineData("favoritos", new[] { "usuario_id", "obra_id", "fecha_marcado" })]
    [InlineData("lecturas", new[] { "id", "usuario_id", "capitulo_id", "fecha_lectura" })]
    [InlineData("comentarios", new[] { "id", "usuario_id", "obra_id", "texto", "fecha" })]
    [InlineData("calificaciones", new[] { "id", "usuario_id", "obra_id", "puntaje", "fecha" })]
    [InlineData("obras_etiquetas", new[] { "obra_id", "etiqueta_id" })]
    public void Las_columnas_de_cada_tabla_son_las_esperadas(string tabla, string[] esperadas)
    {
        Assert.Equal(esperadas.Order(), ColumnasDe(tabla).Order());
    }

    public void Dispose()
    {
        baseDatos.Dispose();
    }
}
