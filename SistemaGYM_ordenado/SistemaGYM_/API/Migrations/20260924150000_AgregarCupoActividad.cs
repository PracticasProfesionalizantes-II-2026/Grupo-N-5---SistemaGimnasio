using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SistemaGYM.Datos;

#nullable disable

namespace SistemaGYM_.Migrations
{
    /// <summary>
    /// Agrega el cupo máximo de alumnos a cada actividad.
    /// Las actividades que ya existían quedan con un cupo de 20 alumnos.
    /// </summary>
    [DbContext(typeof(GimnasioContext))]
    [Migration("20260924150000_AgregarCupoActividad")]
    public partial class AgregarCupoActividad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cupo",
                table: "Actividades",
                type: "int",
                nullable: false,
                defaultValue: 20);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cupo",
                table: "Actividades");
        }
    }
}
