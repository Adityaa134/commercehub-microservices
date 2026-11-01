using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductsService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductQueryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryId",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId_IsActive",
                table: "Products",
                columns: new[] { "CategoryId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsActive_AverageRating_TotalRatings",
                table: "Products",
                columns: new[] { "IsActive", "AverageRating", "TotalRatings" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsActive_CreatedAt",
                table: "Products",
                columns: new[] { "IsActive", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsActive_UnitPrice",
                table: "Products",
                columns: new[] { "IsActive", "UnitPrice" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryId_IsActive",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_IsActive_AverageRating_TotalRatings",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_IsActive_CreatedAt",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_IsActive_UnitPrice",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }
    }
}
