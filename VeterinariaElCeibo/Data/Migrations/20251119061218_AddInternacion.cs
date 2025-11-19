using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeterinariaElCeibo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInternacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultasClinicas_AspNetUsers_VeterinarioId",
                table: "ConsultasClinicas");

            migrationBuilder.AlterColumn<string>(
                name: "VeterinarioId",
                table: "ConsultasClinicas",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "Internaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MascotaId = table.Column<int>(type: "int", nullable: false),
                    ConsultaIngresoId = table.Column<int>(type: "int", nullable: true),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoIngreso = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Internaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Internaciones_ConsultasClinicas_ConsultaIngresoId",
                        column: x => x.ConsultaIngresoId,
                        principalTable: "ConsultasClinicas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Internaciones_Mascotas_MascotaId",
                        column: x => x.MascotaId,
                        principalTable: "Mascotas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosInternacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternacionId = table.Column<int>(type: "int", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    VeterinarioId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PesoKg = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    TemperaturaC = table.Column<decimal>(type: "decimal(4,1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosInternacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosInternacion_AspNetUsers_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RegistrosInternacion_Internaciones_InternacionId",
                        column: x => x.InternacionId,
                        principalTable: "Internaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Internaciones_ConsultaIngresoId",
                table: "Internaciones",
                column: "ConsultaIngresoId");

            migrationBuilder.CreateIndex(
                name: "IX_Internaciones_MascotaId",
                table: "Internaciones",
                column: "MascotaId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosInternacion_InternacionId",
                table: "RegistrosInternacion",
                column: "InternacionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosInternacion_VeterinarioId",
                table: "RegistrosInternacion",
                column: "VeterinarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultasClinicas_AspNetUsers_VeterinarioId",
                table: "ConsultasClinicas",
                column: "VeterinarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultasClinicas_AspNetUsers_VeterinarioId",
                table: "ConsultasClinicas");

            migrationBuilder.DropTable(
                name: "RegistrosInternacion");

            migrationBuilder.DropTable(
                name: "Internaciones");

            migrationBuilder.AlterColumn<string>(
                name: "VeterinarioId",
                table: "ConsultasClinicas",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultasClinicas_AspNetUsers_VeterinarioId",
                table: "ConsultasClinicas",
                column: "VeterinarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
