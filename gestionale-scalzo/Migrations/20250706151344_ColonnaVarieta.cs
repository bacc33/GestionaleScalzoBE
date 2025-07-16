using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestionale_scalzo.Migrations
{
    /// <inheritdoc />
    public partial class ColonnaVarieta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Varietà",
                table: "Orders",
                newName: "Varieta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Varieta",
                table: "Orders",
                newName: "Varietà");
        }
    }
}
