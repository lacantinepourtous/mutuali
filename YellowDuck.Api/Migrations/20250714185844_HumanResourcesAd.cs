using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowDuck.Api.Migrations
{
    public partial class HumanResourcesAd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GeographicCoverage",
                table: "AdTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HumanResourceFieldOther",
                table: "AdTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Qualifications",
                table: "AdTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tasks",
                table: "AdTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HumanResourceField",
                table: "Ads",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeographicCoverage",
                table: "AdTranslations");

            migrationBuilder.DropColumn(
                name: "HumanResourceFieldOther",
                table: "AdTranslations");

            migrationBuilder.DropColumn(
                name: "Qualifications",
                table: "AdTranslations");

            migrationBuilder.DropColumn(
                name: "Tasks",
                table: "AdTranslations");

            migrationBuilder.DropColumn(
                name: "HumanResourceField",
                table: "Ads");
        }
    }
}
