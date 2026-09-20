using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGYM_.Migrations
{
    /// <inheritdoc />
    public partial class PagoBajaLogicaYRestricciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ActividadesAlumno_AlumnoId",
                table: "ActividadesAlumno");

            migrationBuilder.AddColumn<bool>(
                name: "EstaActivo",
                table: "Pagos",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaBaja",
                table: "Pagos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Dni",
                table: "Usuario",
                column: "Dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Email",
                table: "Usuario",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesAlumno_AlumnoId_ActividadId",
                table: "ActividadesAlumno",
                columns: new[] { "AlumnoId", "ActividadId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuario_Dni",
                table: "Usuario");

            migrationBuilder.DropIndex(
                name: "IX_Usuario_Email",
                table: "Usuario");

            migrationBuilder.DropIndex(
                name: "IX_ActividadesAlumno_AlumnoId_ActividadId",
                table: "ActividadesAlumno");

            migrationBuilder.DropColumn(
                name: "EstaActivo",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "FechaBaja",
                table: "Pagos");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesAlumno_AlumnoId",
                table: "ActividadesAlumno",
                column: "AlumnoId");
        }
    }
}
