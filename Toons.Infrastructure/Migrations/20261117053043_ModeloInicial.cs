using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Toons.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModeloInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "etiquetas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_etiquetas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "idiomas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_idiomas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    correo = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    contrasena_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    rol_id = table.Column<int>(type: "integer", nullable: false),
                    intentos_fallidos = table.Column<int>(type: "integer", nullable: false),
                    bloqueado_hasta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuarios", x => x.id);
                    table.ForeignKey(
                        name: "fk_usuarios_roles_rol_id",
                        column: x => x.rol_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "obras",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    portada = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    creador_id = table.Column<int>(type: "integer", nullable: false),
                    idioma_id = table.Column<int>(type: "integer", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_obras", x => x.id);
                    table.ForeignKey(
                        name: "fk_obras_idiomas_idioma_id",
                        column: x => x.idioma_id,
                        principalTable: "idiomas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_obras_usuarios_creador_id",
                        column: x => x.creador_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "calificaciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    obra_id = table.Column<int>(type: "integer", nullable: false),
                    puntaje = table.Column<int>(type: "integer", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_calificaciones", x => x.id);
                    table.CheckConstraint("ck_calificaciones_puntaje", "puntaje BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "fk_calificaciones_obras_obra_id",
                        column: x => x.obra_id,
                        principalTable: "obras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_calificaciones_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "capitulos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    obra_id = table.Column<int>(type: "integer", nullable: false),
                    numero = table.Column<int>(type: "integer", nullable: false),
                    titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    fecha_publicacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_capitulos", x => x.id);
                    table.ForeignKey(
                        name: "fk_capitulos_obras_obra_id",
                        column: x => x.obra_id,
                        principalTable: "obras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comentarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    obra_id = table.Column<int>(type: "integer", nullable: false),
                    texto = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comentarios", x => x.id);
                    table.ForeignKey(
                        name: "fk_comentarios_obras_obra_id",
                        column: x => x.obra_id,
                        principalTable: "obras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_comentarios_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "favoritos",
                columns: table => new
                {
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    obra_id = table.Column<int>(type: "integer", nullable: false),
                    fecha_marcado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_favoritos", x => new { x.usuario_id, x.obra_id });
                    table.ForeignKey(
                        name: "fk_favoritos_obras_obra_id",
                        column: x => x.obra_id,
                        principalTable: "obras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_favoritos_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "obras_etiquetas",
                columns: table => new
                {
                    obra_id = table.Column<int>(type: "integer", nullable: false),
                    etiqueta_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_obras_etiquetas", x => new { x.obra_id, x.etiqueta_id });
                    table.ForeignKey(
                        name: "fk_obras_etiquetas_etiquetas_etiqueta_id",
                        column: x => x.etiqueta_id,
                        principalTable: "etiquetas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_obras_etiquetas_obras_obra_id",
                        column: x => x.obra_id,
                        principalTable: "obras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lecturas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    capitulo_id = table.Column<int>(type: "integer", nullable: false),
                    fecha_lectura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lecturas", x => x.id);
                    table.ForeignKey(
                        name: "fk_lecturas_capitulos_capitulo_id",
                        column: x => x.capitulo_id,
                        principalTable: "capitulos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_lecturas_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "paginas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    capitulo_id = table.Column<int>(type: "integer", nullable: false),
                    numero = table.Column<int>(type: "integer", nullable: false),
                    url_imagen = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_paginas", x => x.id);
                    table.ForeignKey(
                        name: "fk_paginas_capitulos_capitulo_id",
                        column: x => x.capitulo_id,
                        principalTable: "capitulos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_calificaciones_obra_id",
                table: "calificaciones",
                column: "obra_id");

            migrationBuilder.CreateIndex(
                name: "ix_calificaciones_usuario_id_obra_id",
                table: "calificaciones",
                columns: new[] { "usuario_id", "obra_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_capitulos_obra_id_numero",
                table: "capitulos",
                columns: new[] { "obra_id", "numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_comentarios_obra_id",
                table: "comentarios",
                column: "obra_id");

            migrationBuilder.CreateIndex(
                name: "ix_comentarios_usuario_id",
                table: "comentarios",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_etiquetas_nombre",
                table: "etiquetas",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_favoritos_obra_id",
                table: "favoritos",
                column: "obra_id");

            migrationBuilder.CreateIndex(
                name: "ix_idiomas_nombre",
                table: "idiomas",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_lecturas_capitulo_id",
                table: "lecturas",
                column: "capitulo_id");

            migrationBuilder.CreateIndex(
                name: "ix_lecturas_fecha_lectura",
                table: "lecturas",
                column: "fecha_lectura");

            migrationBuilder.CreateIndex(
                name: "ix_lecturas_usuario_id",
                table: "lecturas",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_obras_creador_id",
                table: "obras",
                column: "creador_id");

            migrationBuilder.CreateIndex(
                name: "ix_obras_idioma_id",
                table: "obras",
                column: "idioma_id");

            migrationBuilder.CreateIndex(
                name: "ix_obras_etiquetas_etiqueta_id",
                table: "obras_etiquetas",
                column: "etiqueta_id");

            migrationBuilder.CreateIndex(
                name: "ix_paginas_capitulo_id_numero",
                table: "paginas",
                columns: new[] { "capitulo_id", "numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_roles_nombre",
                table: "roles",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_correo",
                table: "usuarios",
                column: "correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_rol_id",
                table: "usuarios",
                column: "rol_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "calificaciones");

            migrationBuilder.DropTable(
                name: "comentarios");

            migrationBuilder.DropTable(
                name: "favoritos");

            migrationBuilder.DropTable(
                name: "lecturas");

            migrationBuilder.DropTable(
                name: "obras_etiquetas");

            migrationBuilder.DropTable(
                name: "paginas");

            migrationBuilder.DropTable(
                name: "etiquetas");

            migrationBuilder.DropTable(
                name: "capitulos");

            migrationBuilder.DropTable(
                name: "obras");

            migrationBuilder.DropTable(
                name: "idiomas");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
