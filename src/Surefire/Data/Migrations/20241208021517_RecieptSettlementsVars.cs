using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Surefire.Migrations
{
    /// <inheritdoc />
    public partial class RecieptSettlementsVars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FinanceCharge",
                table: "Settlements",
                newName: "PayAmountNeededToBind");

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionAmount",
                table: "Settlements",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FinanceChargePercent",
                table: "Settlements",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FullGrandTotalPayment",
                table: "Settlements",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionAmount",
                table: "Settlements");

            migrationBuilder.DropColumn(
                name: "FinanceChargePercent",
                table: "Settlements");

            migrationBuilder.DropColumn(
                name: "FullGrandTotalPayment",
                table: "Settlements");

            migrationBuilder.RenameColumn(
                name: "PayAmountNeededToBind",
                table: "Settlements",
                newName: "FinanceCharge");
        }
    }
}
