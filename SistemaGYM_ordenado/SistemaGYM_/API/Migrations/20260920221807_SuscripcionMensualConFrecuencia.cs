using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGYM_.Migrations
{
    /// <inheritdoc />
    public partial class SuscripcionMensualConFrecuencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiasPorSemana",
                table: "Suscripciones",
                type: "int",
                nullable: false,
                defaultValue: 1);

            // Los planes existentes usaban DuracionDias para guardar erróneamente la
            // frecuencia semanal. Se conserva ese dato como frecuencia y la vigencia
            // pasa a ser mensual para todos los planes.
            migrationBuilder.Sql(@"
                UPDATE Suscripciones
                SET DiasPorSemana = CASE
                    WHEN DuracionDias BETWEEN 1 AND 7 THEN DuracionDias
                    ELSE 7
                END,
                DuracionDias = 30;

                UPDATE AlumnoSuscripciones
                SET FechaFin = DATEADD(month, 1, FechaInicio)
                WHERE Activa = 1;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiasPorSemana",
                table: "Suscripciones");
        }
    }
}
