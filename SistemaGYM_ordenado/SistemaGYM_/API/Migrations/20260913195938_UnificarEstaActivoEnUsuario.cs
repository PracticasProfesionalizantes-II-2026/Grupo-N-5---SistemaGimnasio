using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGYM_.Migrations
{
    /// <inheritdoc />
    public partial class UnificarEstaActivoEnUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
                UPDATE Usuario
                SET EstaActivo = Alumno_EstaActivo
                WHERE TipoUsuario = 'Alumno';
            ");
            migrationBuilder.DropColumn(
                name: "Alumno_EstaActivo",
                table: "Usuario");

            migrationBuilder.AlterColumn<bool>(
                name: "EstaActivo",
                table: "Usuario",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "EstaActivo",
                table: "Usuario",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<bool>(
                name: "Alumno_EstaActivo",
                table: "Usuario",
                type: "bit",
                nullable: true);
        }
    }
}
