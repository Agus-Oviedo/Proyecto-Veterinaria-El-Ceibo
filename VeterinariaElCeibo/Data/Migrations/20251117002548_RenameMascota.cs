using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeterinariaElCeibo.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameMascota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Mascotas",
                newName: "NombreMascota");

            migrationBuilder.RenameColumn(
                name: "EdadAnios",
                table: "Mascotas",
                newName: "Edad");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NombreMascota",
                table: "Mascotas",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "Edad",
                table: "Mascotas",
                newName: "EdadAnios");
        }
    }
}
