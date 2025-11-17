using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeterinariaElCeibo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVacunacionYDesparasitacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Desparasitaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MascotaId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Producto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VeterinarioId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Desparasitaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Desparasitaciones_AspNetUsers_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Desparasitaciones_Mascotas_MascotaId",
                        column: x => x.MascotaId,
                        principalTable: "Mascotas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vacunaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MascotaId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Vacuna = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VeterinarioId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacunaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vacunaciones_AspNetUsers_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Vacunaciones_Mascotas_MascotaId",
                        column: x => x.MascotaId,
                        principalTable: "Mascotas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Desparasitaciones_MascotaId",
                table: "Desparasitaciones",
                column: "MascotaId");

            migrationBuilder.CreateIndex(
                name: "IX_Desparasitaciones_VeterinarioId",
                table: "Desparasitaciones",
                column: "VeterinarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Vacunaciones_MascotaId",
                table: "Vacunaciones",
                column: "MascotaId");

            migrationBuilder.CreateIndex(
                name: "IX_Vacunaciones_VeterinarioId",
                table: "Vacunaciones",
                column: "VeterinarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Desparasitaciones");

            migrationBuilder.DropTable(
                name: "Vacunaciones");
        }
    }
}
