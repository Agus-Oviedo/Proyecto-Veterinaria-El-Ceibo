using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeterinariaElCeibo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConsultaClinica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConsultasClinicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MascotaId = table.Column<int>(type: "int", nullable: false),
                    TurnoId = table.Column<int>(type: "int", nullable: true),
                    VeterinarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FechaConsulta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Anamnesis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExamenFisico = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PesoKg = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    TemperaturaC = table.Column<decimal>(type: "decimal(4,1)", nullable: true),
                    Diagnostico = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tratamiento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Indicaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProximoControl = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultasClinicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultasClinicas_AspNetUsers_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsultasClinicas_Mascotas_MascotaId",
                        column: x => x.MascotaId,
                        principalTable: "Mascotas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsultasClinicas_Turnos_TurnoId",
                        column: x => x.TurnoId,
                        principalTable: "Turnos",
                        principalColumn: "TurnoId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasClinicas_MascotaId",
                table: "ConsultasClinicas",
                column: "MascotaId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasClinicas_TurnoId",
                table: "ConsultasClinicas",
                column: "TurnoId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasClinicas_VeterinarioId",
                table: "ConsultasClinicas",
                column: "VeterinarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsultasClinicas");
        }
    }
}
