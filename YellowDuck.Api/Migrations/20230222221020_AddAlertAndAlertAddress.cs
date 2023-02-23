using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowDuck.Api.Migrations
{
    public partial class AddAlertAndAlertAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AlertId",
                table: "AdProfessionalKitchenEquipments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AlertAddress",
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
                    Neighborhood = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sublocality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Raw = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertAddress", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Category = table.Column<int>(type: "int", nullable: false),
                    DeliveryTruckType = table.Column<int>(type: "int", nullable: false),
                    Refrigerated = table.Column<bool>(type: "bit", nullable: false),
                    CanSharedRoad = table.Column<bool>(type: "bit", nullable: false),
                    CanHaveDriver = table.Column<bool>(type: "bit", nullable: false),
                    DayAvailability = table.Column<bool>(type: "bit", nullable: false),
                    EveningAvailability = table.Column<bool>(type: "bit", nullable: false),
                    Radius = table.Column<double>(type: "float", nullable: true),
                    AddressId = table.Column<long>(type: "bigint", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    LastSendDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_AlertAddress_AddressId",
                        column: x => x.AddressId,
                        principalTable: "AlertAddress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Alerts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdProfessionalKitchenEquipments_AlertId",
                table: "AdProfessionalKitchenEquipments",
                column: "AlertId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertAddress_Id_Latitude_Longitude",
                table: "AlertAddress",
                columns: new[] { "Id", "Latitude", "Longitude" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_AddressId",
                table: "Alerts",
                column: "AddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_UserId",
                table: "Alerts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdProfessionalKitchenEquipments_Alerts_AlertId",
                table: "AdProfessionalKitchenEquipments",
                column: "AlertId",
                principalTable: "Alerts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdProfessionalKitchenEquipments_Alerts_AlertId",
                table: "AdProfessionalKitchenEquipments");

            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "AlertAddress");

            migrationBuilder.DropIndex(
                name: "IX_AdProfessionalKitchenEquipments_AlertId",
                table: "AdProfessionalKitchenEquipments");

            migrationBuilder.DropColumn(
                name: "AlertId",
                table: "AdProfessionalKitchenEquipments");
        }
    }
}
