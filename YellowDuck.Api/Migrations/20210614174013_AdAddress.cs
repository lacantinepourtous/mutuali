using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowDuck.Api.Migrations
{
    public partial class AdAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AddressId",
                table: "Ads",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "AdAddress",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StreetNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Route = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Locality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Raw = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdAddress", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ads_AddressId",
                table: "Ads",
                column: "AddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdAddress_Id_Latitude_Longitude",
                table: "AdAddress",
                columns: new[] { "Id", "Latitude", "Longitude" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ads_AdAddress_AddressId",
                table: "Ads",
                column: "AddressId",
                principalTable: "AdAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ads_AdAddress_AddressId",
                table: "Ads");

            migrationBuilder.DropTable(
                name: "AdAddress");

            migrationBuilder.DropIndex(
                name: "IX_Ads_AddressId",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Ads");
        }
    }
}
