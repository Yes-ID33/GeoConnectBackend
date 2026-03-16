using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Services.Migrations
{
    /// <inheritdoc />
    public partial class GeoDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Municipios",
                columns: table => new
                {
                    idMunicipio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombreMunicipio = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    departamento = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true, defaultValue: "Antioquia")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Municipi__FD10E400375E83FA", x => x.idMunicipio);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    idUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    correo = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    contrasena = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    verificado = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    tokenVerificacion = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    tokenExpira = table.Column<DateTime>(type: "datetime", nullable: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuarios__645723A627F384E6", x => x.idUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Lugares",
                columns: table => new
                {
                    googlePlaceId = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    idMunicipio = table.Column<int>(type: "int", nullable: true),
                    nombreLugar = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    coordenadas = table.Column<Geometry>(type: "geography", nullable: false),
                    fotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Lugares__FD6A056AB8A76094", x => x.googlePlaceId);
                    table.ForeignKey(
                        name: "FK__Lugares__idMunic__5165187F",
                        column: x => x.idMunicipio,
                        principalTable: "Municipios",
                        principalColumn: "idMunicipio");
                });

            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    idComentario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idUsuario = table.Column<int>(type: "int", nullable: true),
                    googlePlaceId = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    comentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    calificacion = table.Column<int>(type: "int", nullable: false),
                    fechaPublicacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Comentar__C74515DA092105FE", x => x.idComentario);
                    table.ForeignKey(
                        name: "FK__Comentari__googl__5BE2A6F2",
                        column: x => x.googlePlaceId,
                        principalTable: "Lugares",
                        principalColumn: "googlePlaceId");
                    table.ForeignKey(
                        name: "FK__Comentari__idUsu__5AEE82B9",
                        column: x => x.idUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "idUsuario");
                });

            migrationBuilder.CreateTable(
                name: "LugaresAcciones",
                columns: table => new
                {
                    idAccion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idUsuario = table.Column<int>(type: "int", nullable: true),
                    googlePlaceId = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    tipoAccion = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    fechaAccion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LugaresA__E0B207A4DF513223", x => x.idAccion);
                    table.ForeignKey(
                        name: "FK__LugaresAc__googl__5629CD9C",
                        column: x => x.googlePlaceId,
                        principalTable: "Lugares",
                        principalColumn: "googlePlaceId");
                    table.ForeignKey(
                        name: "FK__LugaresAc__idUsu__5535A963",
                        column: x => x.idUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "idUsuario");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_googlePlaceId",
                table: "Comentarios",
                column: "googlePlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_idUsuario",
                table: "Comentarios",
                column: "idUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Lugares_idMunicipio",
                table: "Lugares",
                column: "idMunicipio");

            migrationBuilder.CreateIndex(
                name: "IX_LugaresAcciones_googlePlaceId",
                table: "LugaresAcciones",
                column: "googlePlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_LugaresAcciones_idUsuario",
                table: "LugaresAcciones",
                column: "idUsuario");

            migrationBuilder.CreateIndex(
                name: "UQ__Usuarios__2A586E0B854D0267",
                table: "Usuarios",
                column: "correo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comentarios");

            migrationBuilder.DropTable(
                name: "LugaresAcciones");

            migrationBuilder.DropTable(
                name: "Lugares");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Municipios");
        }
    }
}
