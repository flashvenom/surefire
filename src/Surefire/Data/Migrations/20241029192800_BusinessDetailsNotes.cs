using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Surefire.Migrations
{
    /// <inheritdoc />
    public partial class BusinessDetailsNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessDetails",
                columns: table => new
                {
                    BusinessDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    RecordType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FEIN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LegalEntityType = table.Column<int>(type: "int", nullable: true),
                    ShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LongDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessIndustry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessSpecialty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessType = table.Column<int>(type: "int", nullable: true),
                    EstimatedGrossSales0 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimatedGrossSales1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimatedGrossSales2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimatedGrossSales3 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimatedGrossSales4 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DateStarted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    YearsExperience = table.Column<int>(type: "int", nullable: true),
                    InsuranceHistory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LapseHistory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumClaims = table.Column<int>(type: "int", nullable: true),
                    LicenseType = table.Column<int>(type: "int", nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimatedSubcontractingExpenses = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HqYearBuilt = table.Column<int>(type: "int", nullable: true),
                    HqSquareFootage = table.Column<int>(type: "int", nullable: true),
                    HqNumberOfStories = table.Column<int>(type: "int", nullable: true),
                    HqSprinklered = table.Column<bool>(type: "bit", nullable: true),
                    HqMonitoredSecurity = table.Column<bool>(type: "bit", nullable: true),
                    NumPartTimeEmployees = table.Column<int>(type: "int", nullable: true),
                    NumFullTimeEmployees = table.Column<int>(type: "int", nullable: true),
                    EstimatedAnnualPayroll0 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimatedAnnualPayroll1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimatedAnnualPayroll2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimatedAnnualPayroll3 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimatedAnnualPayroll4 = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessDetails", x => x.BusinessDetailsId);
                    table.ForeignKey(
                        name: "FK_BusinessDetails_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientNotes",
                columns: table => new
                {
                    ClientNoteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientNotes", x => x.ClientNoteId);
                    table.ForeignKey(
                        name: "FK_ClientNotes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessDetails_ClientId",
                table: "BusinessDetails",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientNotes_ClientId",
                table: "ClientNotes",
                column: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessDetails");

            migrationBuilder.DropTable(
                name: "ClientNotes");
        }
    }
}
