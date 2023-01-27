using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowDuck.Api.Migrations
{
    public partial class AddSpecificInformationInAd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryTruckTypeOther",
                table: "AdTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Equipment",
                table: "AdTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfessionalKitchenEquipmentOther",
                table: "AdTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SurfaceDescription",
                table: "AdTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SurfaceSize",
                table: "AdTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CanHaveDriver",
                table: "Ads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanSharedRoad",
                table: "Ads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryTruckType",
                table: "Ads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Refrigerated",
                table: "Ads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AdDayAvailabilityWeekdays",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdId = table.Column<long>(type: "bigint", nullable: false),
                    Weekday = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdDayAvailabilityWeekdays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdDayAvailabilityWeekdays_Ads_AdId",
                        column: x => x.AdId,
                        principalTable: "Ads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdEveningAvailabilityWeekdays",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdId = table.Column<long>(type: "bigint", nullable: false),
                    Weekday = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdEveningAvailabilityWeekdays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdEveningAvailabilityWeekdays_Ads_AdId",
                        column: x => x.AdId,
                        principalTable: "Ads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdProfessionalKitchenEquipments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdId = table.Column<long>(type: "bigint", nullable: false),
                    ProfessionalKitchenEquipment = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdProfessionalKitchenEquipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdProfessionalKitchenEquipments_Ads_AdId",
                        column: x => x.AdId,
                        principalTable: "Ads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdDayAvailabilityWeekdays_AdId",
                table: "AdDayAvailabilityWeekdays",
                column: "AdId");

            migrationBuilder.CreateIndex(
                name: "IX_AdEveningAvailabilityWeekdays_AdId",
                table: "AdEveningAvailabilityWeekdays",
                column: "AdId");

            migrationBuilder.CreateIndex(
                name: "IX_AdProfessionalKitchenEquipments_AdId",
                table: "AdProfessionalKitchenEquipments",
                column: "AdId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdDayAvailabilityWeekdays");

            migrationBuilder.DropTable(
                name: "AdEveningAvailabilityWeekdays");

            migrationBuilder.DropTable(
                name: "AdProfessionalKitchenEquipments");

            migrationBuilder.DropColumn(
                name: "DeliveryTruckTypeOther",
                table: "AdTranslations");

            migrationBuilder.DropColumn(
                name: "Equipment",
                table: "AdTranslations");

            migrationBuilder.DropColumn(
                name: "ProfessionalKitchenEquipmentOther",
                table: "AdTranslations");

            migrationBuilder.DropColumn(
                name: "SurfaceDescription",
                table: "AdTranslations");

            migrationBuilder.DropColumn(
                name: "SurfaceSize",
                table: "AdTranslations");

            migrationBuilder.DropColumn(
                name: "CanHaveDriver",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "CanSharedRoad",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "DeliveryTruckType",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "Refrigerated",
                table: "Ads");
        }
    }
}
