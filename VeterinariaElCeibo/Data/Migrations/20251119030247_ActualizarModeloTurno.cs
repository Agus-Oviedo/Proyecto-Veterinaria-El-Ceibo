using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeterinariaElCeibo.Data.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarModeloTurno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Mascotas_MascotaId",
                table: "Turnos");

            migrationBuilder.AddColumn<string>(
                name: "TipoTurno",
                table: "Turnos",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Mascotas_MascotaId",
                table: "Turnos",
                column: "MascotaId",
                principalTable: "Mascotas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Mascotas_MascotaId",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "TipoTurno",
                table: "Turnos");

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Mascotas_MascotaId",
                table: "Turnos",
                column: "MascotaId",
                principalTable: "Mascotas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
