using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace gestionale_scalzo.Migrations
{
    /// <inheritdoc />
    public partial class Varie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    TaxCode = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Surname = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PIva = table.Column<string>(type: "text", nullable: true),
                    ResidenceAddress = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.TaxCode);
                });

            migrationBuilder.CreateTable(
                name: "Tipologie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipologie", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumeroOrdine = table.Column<string>(type: "text", nullable: true),
                    NumeroColli = table.Column<int>(type: "integer", nullable: false),
                    Varietà = table.Column<string>(type: "text", nullable: true),
                    TipologiaPedaneId = table.Column<int>(type: "integer", nullable: false),
                    NumeroPedane = table.Column<int>(type: "integer", nullable: false),
                    TipologiaCassetteId = table.Column<int>(type: "integer", nullable: false),
                    NumeroCassette = table.Column<int>(type: "integer", nullable: false),
                    PesoCassetta = table.Column<int>(type: "integer", nullable: false),
                    TipologiaBancaleId = table.Column<int>(type: "integer", nullable: false),
                    KiliNetti = table.Column<int>(type: "integer", nullable: false),
                    KiliTotali = table.Column<int>(type: "integer", nullable: false),
                    Cliente = table.Column<string>(type: "text", nullable: true),
                    DataInserimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataPartenza = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PrezzoOrdine = table.Column<int>(type: "integer", nullable: false),
                    CompagniaTrasporto = table.Column<string>(type: "text", nullable: true),
                    Peso = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Clients_Cliente",
                        column: x => x.Cliente,
                        principalTable: "Clients",
                        principalColumn: "TaxCode");
                    table.ForeignKey(
                        name: "FK_Orders_Tipologie_TipologiaBancaleId",
                        column: x => x.TipologiaBancaleId,
                        principalTable: "Tipologie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Tipologie_TipologiaCassetteId",
                        column: x => x.TipologiaCassetteId,
                        principalTable: "Tipologie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Tipologie_TipologiaPedaneId",
                        column: x => x.TipologiaPedaneId,
                        principalTable: "Tipologie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Cliente",
                table: "Orders",
                column: "Cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TipologiaBancaleId",
                table: "Orders",
                column: "TipologiaBancaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TipologiaCassetteId",
                table: "Orders",
                column: "TipologiaCassetteId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TipologiaPedaneId",
                table: "Orders",
                column: "TipologiaPedaneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Tipologie");
        }
    }
}
