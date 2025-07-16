using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestionale_scalzo.Migrations
{
    /// <inheritdoc />
    public partial class ToltoFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Tipologie_TipologiaBancaleId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Tipologie_TipologiaCassetteId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Tipologie_TipologiaPedaneId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TipologiaBancaleId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TipologiaCassetteId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TipologiaPedaneId",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Tipologie_TipologiaBancaleId",
                table: "Orders",
                column: "TipologiaBancaleId",
                principalTable: "Tipologie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Tipologie_TipologiaCassetteId",
                table: "Orders",
                column: "TipologiaCassetteId",
                principalTable: "Tipologie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Tipologie_TipologiaPedaneId",
                table: "Orders",
                column: "TipologiaPedaneId",
                principalTable: "Tipologie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
