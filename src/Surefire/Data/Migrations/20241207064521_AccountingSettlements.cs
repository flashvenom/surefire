using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Surefire.Migrations
{
    /// <inheritdoc />
    public partial class AccountingSettlements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settlements",
                columns: table => new
                {
                    SettlementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFinanced = table.Column<bool>(type: "bit", nullable: false),
                    FinanceMonths = table.Column<int>(type: "int", nullable: true),
                    FinanceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FinanceCharge = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FinanceAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayAmountNet = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PayFees = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PayNet = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PayDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PayNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayDepositDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsFullPayment = table.Column<bool>(type: "bit", nullable: false),
                    DownPaymentPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CommissionPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountingIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccountingPaidToCarrierAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AccountingPaidOnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccountingStatementNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RenewalId = table.Column<int>(type: "int", nullable: true),
                    PolicyId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settlements", x => x.SettlementId);
                    table.ForeignKey(
                        name: "FK_Settlements_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Settlements_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Settlements_Renewals_RenewalId",
                        column: x => x.RenewalId,
                        principalTable: "Renewals",
                        principalColumn: "RenewalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SettlementItems",
                columns: table => new
                {
                    SettlementItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SettlementId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettlementItems", x => x.SettlementItemId);
                    table.ForeignKey(
                        name: "FK_SettlementItems_Settlements_SettlementId",
                        column: x => x.SettlementId,
                        principalTable: "Settlements",
                        principalColumn: "SettlementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SettlementItems_SettlementId",
                table: "SettlementItems",
                column: "SettlementId");

            migrationBuilder.CreateIndex(
                name: "IX_Settlements_CreatedById",
                table: "Settlements",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Settlements_PolicyId",
                table: "Settlements",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Settlements_RenewalId",
                table: "Settlements",
                column: "RenewalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SettlementItems");

            migrationBuilder.DropTable(
                name: "Settlements");
        }
    }
}
