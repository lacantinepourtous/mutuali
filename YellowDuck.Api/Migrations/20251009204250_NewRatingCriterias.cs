using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowDuck.Api.Migrations
{
    public partial class NewRatingCriterias : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdRatings_Ads_AdId",
                table: "AdRatings");

            migrationBuilder.RenameColumn(
                name: "FiabilityRating",
                table: "UserRatings",
                newName: "OverallRating");

            migrationBuilder.RenameColumn(
                name: "SecurityRating",
                table: "AdRatings",
                newName: "QualityRating");

            migrationBuilder.RenameColumn(
                name: "CleanlinessRating",
                table: "AdRatings",
                newName: "OverallRating");

            migrationBuilder.AddForeignKey(
                name: "FK_AdRatings_Ads_AdId",
                table: "AdRatings",
                column: "AdId",
                principalTable: "Ads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdRatings_Ads_AdId",
                table: "AdRatings");

            migrationBuilder.RenameColumn(
                name: "OverallRating",
                table: "UserRatings",
                newName: "FiabilityRating");

            migrationBuilder.RenameColumn(
                name: "QualityRating",
                table: "AdRatings",
                newName: "SecurityRating");

            migrationBuilder.RenameColumn(
                name: "OverallRating",
                table: "AdRatings",
                newName: "CleanlinessRating");

            migrationBuilder.AddForeignKey(
                name: "FK_AdRatings_Ads_AdId",
                table: "AdRatings",
                column: "AdId",
                principalTable: "Ads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
