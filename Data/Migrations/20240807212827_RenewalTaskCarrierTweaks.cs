using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class RenewalTaskCarrierTweaks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Renewals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LineCode",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LineNickname",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CarrierNickname",
                table: "Carriers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Renewals_ProductId",
                table: "Renewals",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Renewals_Products_ProductId",
                table: "Renewals",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Renewals_Products_ProductId",
                table: "Renewals");

            migrationBuilder.DropIndex(
                name: "IX_Renewals_ProductId",
                table: "Renewals");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Renewals");

            migrationBuilder.DropColumn(
                name: "LineCode",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LineNickname",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CarrierNickname",
                table: "Carriers");
        }
    }
}
