using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class AddingPropertyUmbrellaCoverageModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PropertyCoverage",
                columns: table => new
                {
                    PropertyCoverageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessPersonalProperty = table.Column<int>(type: "int", nullable: true),
                    Equipment = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PolicyId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyCoverage", x => x.PropertyCoverageId);
                    table.ForeignKey(
                        name: "FK_PropertyCoverage_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyCoverage_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyCoverage_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyCoverage_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UmbrellaCoverage",
                columns: table => new
                {
                    UmbrellaCoverageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsUmbrella = table.Column<bool>(type: "bit", nullable: true),
                    IsExcess = table.Column<bool>(type: "bit", nullable: true),
                    ClaimsMade = table.Column<bool>(type: "bit", nullable: true),
                    Occurrence = table.Column<bool>(type: "bit", nullable: true),
                    HasRetention = table.Column<bool>(type: "bit", nullable: true),
                    HasDeductible = table.Column<bool>(type: "bit", nullable: true),
                    EachOccurrence = table.Column<int>(type: "int", nullable: true),
                    GeneralAggregate = table.Column<int>(type: "int", nullable: true),
                    DeductibleRetentionAmount = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PolicyId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UmbrellaCoverage", x => x.UmbrellaCoverageId);
                    table.ForeignKey(
                        name: "FK_UmbrellaCoverage_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UmbrellaCoverage_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UmbrellaCoverage_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UmbrellaCoverage_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCoverage_ClientId",
                table: "PropertyCoverage",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCoverage_CreatedById",
                table: "PropertyCoverage",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCoverage_ModifiedById",
                table: "PropertyCoverage",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCoverage_PolicyId",
                table: "PropertyCoverage",
                column: "PolicyId",
                unique: true,
                filter: "[PolicyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UmbrellaCoverage_ClientId",
                table: "UmbrellaCoverage",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_UmbrellaCoverage_CreatedById",
                table: "UmbrellaCoverage",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UmbrellaCoverage_ModifiedById",
                table: "UmbrellaCoverage",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_UmbrellaCoverage_PolicyId",
                table: "UmbrellaCoverage",
                column: "PolicyId",
                unique: true,
                filter: "[PolicyId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyCoverage");

            migrationBuilder.DropTable(
                name: "UmbrellaCoverage");
        }
    }
}
