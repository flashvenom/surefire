using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class MoreBusinessDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HqYearBuilt",
                table: "BusinessDetails",
                newName: "BuildingLocationYearBuilt");

            migrationBuilder.RenameColumn(
                name: "HqSquareFootage",
                table: "BusinessDetails",
                newName: "BuildingLocationSquareFootage");

            migrationBuilder.RenameColumn(
                name: "HqSprinklered",
                table: "BusinessDetails",
                newName: "BuildingLocationSprinklered");

            migrationBuilder.RenameColumn(
                name: "HqNumberOfStories",
                table: "BusinessDetails",
                newName: "BuildingLocationNumberOfStories");

            migrationBuilder.RenameColumn(
                name: "HqMonitoredSecurity",
                table: "BusinessDetails",
                newName: "BuildingLocationMonitoredSecurity");

            migrationBuilder.AddColumn<decimal>(
                name: "AnnualGrossSalesRevenueReceipts",
                table: "BusinessDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AnnualPayrollHazardExposure",
                table: "BusinessDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BusinessPersonalPropertyBPP",
                table: "BusinessDetails",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnnualGrossSalesRevenueReceipts",
                table: "BusinessDetails");

            migrationBuilder.DropColumn(
                name: "AnnualPayrollHazardExposure",
                table: "BusinessDetails");

            migrationBuilder.DropColumn(
                name: "BusinessPersonalPropertyBPP",
                table: "BusinessDetails");

            migrationBuilder.RenameColumn(
                name: "BuildingLocationYearBuilt",
                table: "BusinessDetails",
                newName: "HqYearBuilt");

            migrationBuilder.RenameColumn(
                name: "BuildingLocationSquareFootage",
                table: "BusinessDetails",
                newName: "HqSquareFootage");

            migrationBuilder.RenameColumn(
                name: "BuildingLocationSprinklered",
                table: "BusinessDetails",
                newName: "HqSprinklered");

            migrationBuilder.RenameColumn(
                name: "BuildingLocationNumberOfStories",
                table: "BusinessDetails",
                newName: "HqNumberOfStories");

            migrationBuilder.RenameColumn(
                name: "BuildingLocationMonitoredSecurity",
                table: "BusinessDetails",
                newName: "HqMonitoredSecurity");
        }
    }
}
