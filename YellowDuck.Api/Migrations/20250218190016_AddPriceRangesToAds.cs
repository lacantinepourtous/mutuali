using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowDuck.Api.Migrations
{
    public partial class AddPriceRangesToAds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RentPriceRange",
                table: "Ads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SalePriceRange",
                table: "Ads",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RentPriceRange",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "SalePriceRange",
                table: "Ads");
        }
    }
}
