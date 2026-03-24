using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Services.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseFinalVersion : Migration
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
                    nombreMunicipio = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    departamento = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true, defaultValue: "Antioquia")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipios", x => x.idMunicipio);
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
                    table.PrimaryKey("PK_Usuarios", x => x.idUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Lugares",
                columns: table => new
                {
                    idLugar = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    googlePlaceId = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    idMunicipio = table.Column<int>(type: "int", nullable: true),
                    nombreLugar = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    coordenadas = table.Column<Geometry>(type: "geography", nullable: true),
                    fotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lugares", x => x.idLugar);
                    table.ForeignKey(
                        name: "FK_Lugares_Municipios",
                        column: x => x.idMunicipio,
                        principalTable: "Municipios",
                        principalColumn: "idMunicipio");
                });

            migrationBuilder.CreateTable(
                name: "AccionLugar",
                columns: table => new
                {
                    idAccion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idUsuario = table.Column<int>(type: "int", nullable: false),
                    idLugar = table.Column<int>(type: "int", nullable: false),
                    tipoAccion = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    fechaAccion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccionLugar", x => x.idAccion);
                    table.ForeignKey(
                        name: "FK_AccionLugar_Lugares",
                        column: x => x.idLugar,
                        principalTable: "Lugares",
                        principalColumn: "idLugar",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccionLugar_Usuarios",
                        column: x => x.idUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "idUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    idComentario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idUsuario = table.Column<int>(type: "int", nullable: false),
                    idLugar = table.Column<int>(type: "int", nullable: false),
                    comentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    calificacion = table.Column<float>(type: "real", nullable: false),
                    fechaPublicacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.idComentario);
                    table.CheckConstraint("CK_Comentario_Calificacion", "calificacion >= 1 AND calificacion <= 5");
                    table.ForeignKey(
                        name: "FK_Comentarios_Lugares",
                        column: x => x.idLugar,
                        principalTable: "Lugares",
                        principalColumn: "idLugar",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comentarios_Usuarios",
                        column: x => x.idUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "idUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccionLugar_idLugar",
                table: "AccionLugar",
                column: "idLugar");

            migrationBuilder.CreateIndex(
                name: "IX_AccionLugar_idUsuario",
                table: "AccionLugar",
                column: "idUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_idLugar",
                table: "Comentarios",
                column: "idLugar");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_idUsuario",
                table: "Comentarios",
                column: "idUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Lugares_googlePlaceId",
                table: "Lugares",
                column: "googlePlaceId",
                unique: true,
                filter: "[googlePlaceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Lugares_idMunicipio",
                table: "Lugares",
                column: "idMunicipio");

            migrationBuilder.CreateIndex(
                name: "UQ_Usuarios_Correo",
                table: "Usuarios",
                column: "correo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccionLugar");

            migrationBuilder.DropTable(
                name: "Comentarios");

            migrationBuilder.DropTable(
                name: "Lugares");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Municipios");
        }
    }
}
