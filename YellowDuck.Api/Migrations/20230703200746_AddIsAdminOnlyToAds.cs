using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowDuck.Api.Migrations
{
    public partial class AddIsAdminOnlyToAds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdminOnly",
                table: "Ads",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdminOnly",
                table: "Ads");
        }
    }
}
