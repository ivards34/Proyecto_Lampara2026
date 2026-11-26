using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class ConsolidateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clima_lecturas");

            migrationBuilder.RenameColumn(
                name: "Temperatura",
                table: "lampara_lecturas",
                newName: "TemperaturaSensor");

            migrationBuilder.RenameColumn(
                name: "Lampara",
                table: "lampara_lecturas",
                newName: "EstadoLampara");

            migrationBuilder.RenameColumn(
                name: "Humedad",
                table: "lampara_lecturas",
                newName: "HumedadSensor");

            migrationBuilder.RenameColumn(
                name: "Dispositivo",
                table: "lampara_lecturas",
                newName: "DispositivoId");

            migrationBuilder.AddColumn<double>(
                name: "TemperaturaApi",
                table: "lampara_lecturas",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "HumedadApi",
                table: "lampara_lecturas",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HumedadApi",
                table: "lampara_lecturas");

            migrationBuilder.DropColumn(
                name: "TemperaturaApi",
                table: "lampara_lecturas");

            migrationBuilder.RenameColumn(
                name: "TemperaturaSensor",
                table: "lampara_lecturas",
                newName: "Temperatura");

            migrationBuilder.RenameColumn(
                name: "HumedadSensor",
                table: "lampara_lecturas",
                newName: "Humedad");

            migrationBuilder.RenameColumn(
                name: "EstadoLampara",
                table: "lampara_lecturas",
                newName: "Lampara");

            migrationBuilder.RenameColumn(
                name: "DispositivoId",
                table: "lampara_lecturas",
                newName: "Dispositivo");

            migrationBuilder.CreateTable(
                name: "clima_lecturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FechaHora = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Humedad = table.Column<double>(type: "double precision", nullable: false),
                    Temperatura = table.Column<double>(type: "double precision", nullable: false),
                    Ubicacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clima_lecturas", x => x.Id);
                });
        }
    }
}
