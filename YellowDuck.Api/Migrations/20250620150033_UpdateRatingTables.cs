using Microsoft.EntityFrameworkCore.Migrations;

namespace YellowDuck.Api.Migrations
{
    public partial class UpdateRatingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdRatings_Ads_AdId",
                table: "AdRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_AdRatings_Contracts_ContractId",
                table: "AdRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRatings_Contracts_ContractId",
                table: "UserRatings");

            migrationBuilder.DropIndex(
                name: "IX_AdRatings_ContractId",
                table: "AdRatings");

            migrationBuilder.RenameColumn(
                name: "ContractId",
                table: "UserRatings",
                newName: "ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRatings_ContractId",
                table: "UserRatings",
                newName: "IX_UserRatings_ConversationId");

            migrationBuilder.RenameColumn(
                name: "ContractId",
                table: "AdRatings",
                newName: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_AdRatings_ConversationId",
                table: "AdRatings",
                column: "ConversationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdRatings_Ads_AdId",
                table: "AdRatings",
                column: "AdId",
                principalTable: "Ads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AdRatings_Conversations_ConversationId",
                table: "AdRatings",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRatings_Conversations_ConversationId",
                table: "UserRatings",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdRatings_Ads_AdId",
                table: "AdRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_AdRatings_Conversations_ConversationId",
                table: "AdRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRatings_Conversations_ConversationId",
                table: "UserRatings");

            migrationBuilder.DropIndex(
                name: "IX_AdRatings_ConversationId",
                table: "AdRatings");

            migrationBuilder.RenameColumn(
                name: "ConversationId",
                table: "UserRatings",
                newName: "ContractId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRatings_ConversationId",
                table: "UserRatings",
                newName: "IX_UserRatings_ContractId");

            migrationBuilder.RenameColumn(
                name: "ConversationId",
                table: "AdRatings",
                newName: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_AdRatings_ContractId",
                table: "AdRatings",
                column: "ContractId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AdRatings_Ads_AdId",
                table: "AdRatings",
                column: "AdId",
                principalTable: "Ads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdRatings_Contracts_ContractId",
                table: "AdRatings",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRatings_Contracts_ContractId",
                table: "UserRatings",
                column: "ContractId",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
