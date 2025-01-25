using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Surefire.Migrations
{
    /// <inheritdoc />
    public partial class SettlementJsonAiFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountingBillingCompany",
                table: "Settlements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountingCarrier",
                table: "Settlements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountingPolicyNumber",
                table: "Settlements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AccountingStatementDueDate",
                table: "Settlements",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountingBillingCompany",
                table: "Settlements");

            migrationBuilder.DropColumn(
                name: "AccountingCarrier",
                table: "Settlements");

            migrationBuilder.DropColumn(
                name: "AccountingPolicyNumber",
                table: "Settlements");

            migrationBuilder.DropColumn(
                name: "AccountingStatementDueDate",
                table: "Settlements");
        }
    }
}
