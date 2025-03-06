using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowDuck.Api.Migrations
{
    public partial class AddCertificationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdCertifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdId = table.Column<long>(type: "bigint", nullable: false),
                    Certification = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdCertifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdCertifications_Ads_AdId",
                        column: x => x.AdId,
                        principalTable: "Ads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdCertifications_AdId",
                table: "AdCertifications",
                column: "AdId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdCertifications");
        }
    }
}
