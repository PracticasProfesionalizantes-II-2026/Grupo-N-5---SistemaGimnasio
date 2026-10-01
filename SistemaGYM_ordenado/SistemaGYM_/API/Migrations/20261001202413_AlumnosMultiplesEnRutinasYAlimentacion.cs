using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGYM_.Migrations
{
    /// <summary>
    /// Una rutina o un plan de alimentación ahora puede estar asignado a varios alumnos
    /// (o a ninguno, y entonces es general). Se reemplaza la columna AlumnoId por las
    /// tablas intermedias RutinaAlumnos y AlimentacionAlumnos.
    /// El orden importa: primero se crean las tablas nuevas, después se copian los
    /// alumnos que ya estaban asignados y recién al final se borran las columnas viejas,
    /// así no se pierde ninguna asignación.
    /// </summary>
    public partial class AlumnosMultiplesEnRutinasYAlimentacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Tablas intermedias nuevas
            migrationBuilder.CreateTable(
                name: "AlimentacionAlumnos",
                columns: table => new
                {
                    AlimentacionId = table.Column<int>(type: "int", nullable: false),
                    AlumnoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlimentacionAlumnos", x => new { x.AlimentacionId, x.AlumnoId });
                    table.ForeignKey(
                        name: "FK_AlimentacionAlumnos_Alimentaciones_AlimentacionId",
                        column: x => x.AlimentacionId,
                        principalTable: "Alimentaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlimentacionAlumnos_Usuario_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RutinaAlumnos",
                columns: table => new
                {
                    RutinaId = table.Column<int>(type: "int", nullable: false),
                    AlumnoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RutinaAlumnos", x => new { x.RutinaId, x.AlumnoId });
                    table.ForeignKey(
                        name: "FK_RutinaAlumnos_Rutinas_RutinaId",
                        column: x => x.RutinaId,
                        principalTable: "Rutinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RutinaAlumnos_Usuario_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlimentacionAlumnos_AlumnoId",
                table: "AlimentacionAlumnos",
                column: "AlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_RutinaAlumnos_AlumnoId",
                table: "RutinaAlumnos",
                column: "AlumnoId");

            // 2) Se copian las asignaciones que ya existían.
            //    Los planes de alimentación sin alumno eran generales y siguen siéndolo (no llevan filas).
            migrationBuilder.Sql(
                "INSERT INTO RutinaAlumnos (RutinaId, AlumnoId) SELECT Id, AlumnoId FROM Rutinas;");
            migrationBuilder.Sql(
                "INSERT INTO AlimentacionAlumnos (AlimentacionId, AlumnoId) SELECT Id, AlumnoId FROM Alimentaciones WHERE AlumnoId IS NOT NULL;");

            // 3) Recién ahora se borran las columnas viejas
            migrationBuilder.DropForeignKey(
                name: "FK_Alimentaciones_Usuario_AlumnoId",
                table: "Alimentaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutinas_Usuario_AlumnoId",
                table: "Rutinas");

            migrationBuilder.DropIndex(
                name: "IX_Rutinas_AlumnoId",
                table: "Rutinas");

            migrationBuilder.DropIndex(
                name: "IX_Alimentaciones_AlumnoId",
                table: "Alimentaciones");

            migrationBuilder.DropColumn(
                name: "AlumnoId",
                table: "Rutinas");

            migrationBuilder.DropColumn(
                name: "AlumnoId",
                table: "Alimentaciones");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Vuelta atrás: cada rutina/plan recupera un solo alumno (el de menor Id).
            // Las rutinas generales quedan con AlumnoId vacío, por eso la columna vuelve como nullable.
            migrationBuilder.AddColumn<int>(
                name: "AlumnoId",
                table: "Rutinas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AlumnoId",
                table: "Alimentaciones",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE r SET AlumnoId = (SELECT MIN(ra.AlumnoId) FROM RutinaAlumnos ra WHERE ra.RutinaId = r.Id) FROM Rutinas r;");
            migrationBuilder.Sql(
                "UPDATE a SET AlumnoId = (SELECT MIN(aa.AlumnoId) FROM AlimentacionAlumnos aa WHERE aa.AlimentacionId = a.Id) FROM Alimentaciones a;");

            migrationBuilder.DropTable(
                name: "AlimentacionAlumnos");

            migrationBuilder.DropTable(
                name: "RutinaAlumnos");

            migrationBuilder.CreateIndex(
                name: "IX_Rutinas_AlumnoId",
                table: "Rutinas",
                column: "AlumnoId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Rutinas_Usuario_AlumnoId",
                table: "Rutinas",
                column: "AlumnoId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
