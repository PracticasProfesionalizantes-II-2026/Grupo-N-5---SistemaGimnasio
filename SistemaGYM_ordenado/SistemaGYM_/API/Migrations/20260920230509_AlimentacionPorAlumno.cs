using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGYM_.Migrations
{
    /// <inheritdoc />
    public partial class AlimentacionPorAlumno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AlumnoId",
                table: "Alimentaciones",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alimentaciones_AlumnoId",
                table: "Alimentaciones",
                column: "AlumnoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alimentaciones_Usuario_AlumnoId",
                table: "Alimentaciones",
                column: "AlumnoId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alimentaciones_Usuario_AlumnoId",
                table: "Alimentaciones");

            migrationBuilder.DropIndex(
                name: "IX_Alimentaciones_AlumnoId",
                table: "Alimentaciones");

            migrationBuilder.DropColumn(
                name: "AlumnoId",
                table: "Alimentaciones");
        }
    }
}
