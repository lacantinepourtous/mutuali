using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowDuck.Api.Migrations
{
    public partial class AddModalClosedOnUserProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FirstLoginModalClosed",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstLoginModalClosed",
                table: "AspNetUsers");
        }
    }
}
