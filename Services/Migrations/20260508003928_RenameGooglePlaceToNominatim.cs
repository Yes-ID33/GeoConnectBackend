using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Migrations
{
    /// <inheritdoc />
    public partial class RenameGooglePlaceToNominatim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lugares_googlePlaceId",
                table: "Lugares");

            migrationBuilder.RenameColumn(
                name: "googlePlaceId",
                table: "Lugares",
                newName: "NominatimId");

            migrationBuilder.CreateIndex(
                name: "IX_Lugares_NominatimId",
                table: "Lugares",
                column: "NominatimId",
                unique: true,
                filter: "[NominatimId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lugares_NominatimId",
                table: "Lugares");

            migrationBuilder.RenameColumn(
                name: "NominatimId",
                table: "Lugares",
                newName: "googlePlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Lugares_googlePlaceId",
                table: "Lugares",
                column: "googlePlaceId",
                unique: true,
                filter: "[googlePlaceId] IS NOT NULL");
        }
    }
}
