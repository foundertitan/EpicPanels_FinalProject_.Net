using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EpicPanels_Group5_FinalProject.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReviewq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerReviews_Products_ProductID1",
                table: "CustomerReviews");

            migrationBuilder.DropIndex(
                name: "IX_CustomerReviews_ProductID1",
                table: "CustomerReviews");

            migrationBuilder.DropColumn(
                name: "ProductID1",
                table: "CustomerReviews");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductID1",
                table: "CustomerReviews",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReviews_ProductID1",
                table: "CustomerReviews",
                column: "ProductID1");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerReviews_Products_ProductID1",
                table: "CustomerReviews",
                column: "ProductID1",
                principalTable: "Products",
                principalColumn: "ProductID");
        }
    }
}
