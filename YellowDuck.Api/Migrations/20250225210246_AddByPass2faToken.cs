using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowDuck.Api.Migrations
{
    public partial class AddByPass2faToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Bypass2FAExpirationUtc",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bypass2FAToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bypass2FAExpirationUtc",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Bypass2FAToken",
                table: "AspNetUsers");
        }
    }
}
