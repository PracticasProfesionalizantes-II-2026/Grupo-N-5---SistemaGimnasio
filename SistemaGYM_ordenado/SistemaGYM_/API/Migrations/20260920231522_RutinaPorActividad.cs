using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGYM_.Migrations
{
    /// <inheritdoc />
    public partial class RutinaPorActividad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActividadId",
                table: "Rutinas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rutinas_ActividadId",
                table: "Rutinas",
                column: "ActividadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rutinas_Actividades_ActividadId",
                table: "Rutinas",
                column: "ActividadId",
                principalTable: "Actividades",
                principalColumn: "ActividadId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rutinas_Actividades_ActividadId",
                table: "Rutinas");

            migrationBuilder.DropIndex(
                name: "IX_Rutinas_ActividadId",
                table: "Rutinas");

            migrationBuilder.DropColumn(
                name: "ActividadId",
                table: "Rutinas");
        }
    }
}
