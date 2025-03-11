using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowDuck.Api.Migrations
{
    public partial class RentSaleTradeDonationOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TosAcceptationIpAddress",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "PriceDescription",
                table: "AdTranslations",
                newName: "RentPriceDescription");

            migrationBuilder.RenameColumn(
                name: "PriceToBeDetermined",
                table: "Ads",
                newName: "RentPriceToBeDetermined");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Ads",
                newName: "RentPrice");

            migrationBuilder.AddColumn<string>(
                name: "DonationDescription",
                table: "AdTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TradeDescription",
                table: "AdTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalePriceDescription",
                table: "AdTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailableForDonation",
                table: "Ads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailableForRent",
                table: "Ads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailableForSale",
                table: "Ads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailableForTrade",
                table: "Ads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "SalePrice",
                table: "Ads",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SalePriceToBeDetermined",
                table: "Ads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Mettre à jour la colonne IsAvailableForRent pour les enregistrements existants
            migrationBuilder.Sql("UPDATE Ads SET IsAvailableForRent = 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DonationDescription",
                table: "AdTranslations");

            migrationBuilder.DropColumn(
                name: "TradeDescription",
                table: "AdTranslations");

            migrationBuilder.DropColumn(
                name: "SalePriceDescription",
                table: "AdTranslations");

            migrationBuilder.DropColumn(
                name: "IsAvailableForDonation",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "IsAvailableForRent",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "IsAvailableForSale",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "IsAvailableForTrade",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "SalePrice",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "SalePriceToBeDetermined",
                table: "Ads");

            migrationBuilder.RenameColumn(
                name: "RentPriceDescription",
                table: "AdTranslations",
                newName: "PriceDescription");

            migrationBuilder.RenameColumn(
                name: "RentPriceToBeDetermined",
                table: "Ads",
                newName: "PriceToBeDetermined");

            migrationBuilder.RenameColumn(
                name: "RentPrice",
                table: "Ads",
                newName: "Price");

            migrationBuilder.AddColumn<string>(
                name: "TosAcceptationIpAddress",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
