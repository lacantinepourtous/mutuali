using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowDuck.Api.Migrations
{
    public partial class AdRatingsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdRatings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<long>(type: "bigint", nullable: false),
                    AdId = table.Column<long>(type: "bigint", nullable: false),
                    RaterUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ComplianceRating = table.Column<int>(type: "int", nullable: false),
                    CleanlinessRating = table.Column<int>(type: "int", nullable: false),
                    SecurityRating = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdRatings_Ads_AdId",
                        column: x => x.AdId,
                        principalTable: "Ads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdRatings_AspNetUsers_RaterUserId",
                        column: x => x.RaterUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdRatings_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdRatings_AdId",
                table: "AdRatings",
                column: "AdId");

            migrationBuilder.CreateIndex(
                name: "IX_AdRatings_ContractId",
                table: "AdRatings",
                column: "ContractId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdRatings_RaterUserId",
                table: "AdRatings",
                column: "RaterUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdRatings");
        }
    }
}
