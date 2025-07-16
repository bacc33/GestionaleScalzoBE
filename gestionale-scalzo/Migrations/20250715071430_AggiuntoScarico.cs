using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestionale_scalzo.Migrations
{
    /// <inheritdoc />
    public partial class AggiuntoScarico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Scarico",
                table: "Orders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Scarico",
                table: "Orders");
        }
    }
}
