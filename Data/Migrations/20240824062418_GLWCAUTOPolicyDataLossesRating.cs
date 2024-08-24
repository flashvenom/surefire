using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class GLWCAUTOPolicyDataLossesRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BuildingTotalSquareFootage",
                table: "Locations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GrossSales",
                table: "Locations",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumFullTimeEmployees",
                table: "Locations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumPartTimeEmployees",
                table: "Locations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumStories",
                table: "Locations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OccupiedSquareFootage",
                table: "Locations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Owner",
                table: "Locations",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Tenant",
                table: "Locations",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AutoCoverages",
                columns: table => new
                {
                    AutoCoverageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CombinedLimit = table.Column<int>(type: "int", nullable: true),
                    BodilyInjuryPerPerson = table.Column<int>(type: "int", nullable: true),
                    BodilyInjuryPerAccident = table.Column<int>(type: "int", nullable: true),
                    PropertyDamage = table.Column<int>(type: "int", nullable: true),
                    AdditionalCoverageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalCoverageLimit = table.Column<int>(type: "int", nullable: true),
                    ForAny = table.Column<bool>(type: "bit", nullable: true),
                    ForOwned = table.Column<bool>(type: "bit", nullable: true),
                    ForHired = table.Column<bool>(type: "bit", nullable: true),
                    ForScheduled = table.Column<bool>(type: "bit", nullable: true),
                    ForNonOwned = table.Column<bool>(type: "bit", nullable: true),
                    AdditionalInsured = table.Column<bool>(type: "bit", nullable: true),
                    AdditionalInsuredAttachmentAttachmentId = table.Column<int>(type: "int", nullable: true),
                    WaiverOfSub = table.Column<bool>(type: "bit", nullable: true),
                    WaiverOfSubAttachmentAttachmentId = table.Column<int>(type: "int", nullable: true),
                    AdditionalAttachments = table.Column<bool>(type: "bit", nullable: true),
                    AdditionalAttachmentsAttachmentAttachmentId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    PolicyId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoCoverages", x => x.AutoCoverageId);
                    table.ForeignKey(
                        name: "FK_AutoCoverages_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AutoCoverages_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AutoCoverages_Attachments_AdditionalAttachmentsAttachmentAttachmentId",
                        column: x => x.AdditionalAttachmentsAttachmentAttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "AttachmentId");
                    table.ForeignKey(
                        name: "FK_AutoCoverages_Attachments_AdditionalInsuredAttachmentAttachmentId",
                        column: x => x.AdditionalInsuredAttachmentAttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "AttachmentId");
                    table.ForeignKey(
                        name: "FK_AutoCoverages_Attachments_WaiverOfSubAttachmentAttachmentId",
                        column: x => x.WaiverOfSubAttachmentAttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "AttachmentId");
                    table.ForeignKey(
                        name: "FK_AutoCoverages_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AutoCoverages_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GeneralLiabilityCoverages",
                columns: table => new
                {
                    GeneralLiabilityCoverageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EachOccurrence = table.Column<int>(type: "int", nullable: true),
                    DamageToPremises = table.Column<int>(type: "int", nullable: true),
                    MedicalExpenses = table.Column<int>(type: "int", nullable: true),
                    PersonalInjury = table.Column<int>(type: "int", nullable: true),
                    GeneralAggregate = table.Column<int>(type: "int", nullable: true),
                    ProductsAggregate = table.Column<int>(type: "int", nullable: true),
                    AdditionalCoverageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalCoverageLimit = table.Column<int>(type: "int", nullable: true),
                    Premium = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ClaimsMade = table.Column<bool>(type: "bit", nullable: true),
                    Occurence = table.Column<bool>(type: "bit", nullable: true),
                    AggregateAppliesPer = table.Column<int>(type: "int", nullable: true),
                    AdditionalInsured = table.Column<bool>(type: "bit", nullable: true),
                    AdditionalInsuredAttachmentAttachmentId = table.Column<int>(type: "int", nullable: true),
                    WaiverOfSub = table.Column<bool>(type: "bit", nullable: true),
                    WaiverOfSubAttachmentAttachmentId = table.Column<int>(type: "int", nullable: true),
                    AdditionalAttachments = table.Column<bool>(type: "bit", nullable: true),
                    AdditionalAttachmentsAttachmentAttachmentId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PolicyId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralLiabilityCoverages", x => x.GeneralLiabilityCoverageId);
                    table.ForeignKey(
                        name: "FK_GeneralLiabilityCoverages_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeneralLiabilityCoverages_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeneralLiabilityCoverages_Attachments_AdditionalAttachmentsAttachmentAttachmentId",
                        column: x => x.AdditionalAttachmentsAttachmentAttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "AttachmentId");
                    table.ForeignKey(
                        name: "FK_GeneralLiabilityCoverages_Attachments_AdditionalInsuredAttachmentAttachmentId",
                        column: x => x.AdditionalInsuredAttachmentAttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "AttachmentId");
                    table.ForeignKey(
                        name: "FK_GeneralLiabilityCoverages_Attachments_WaiverOfSubAttachmentAttachmentId",
                        column: x => x.WaiverOfSubAttachmentAttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "AttachmentId");
                    table.ForeignKey(
                        name: "FK_GeneralLiabilityCoverages_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeneralLiabilityCoverages_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Losses",
                columns: table => new
                {
                    LossId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateOccurred = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LongDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateClaimSubmitted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AmountReserved = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Subgrogated = table.Column<bool>(type: "bit", nullable: true),
                    Open = table.Column<bool>(type: "bit", nullable: true),
                    UserModifiedId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PolicyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Losses", x => x.LossId);
                    table.ForeignKey(
                        name: "FK_Losses_AspNetUsers_UserModifiedId",
                        column: x => x.UserModifiedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Losses_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RatingBases",
                columns: table => new
                {
                    RatingBasisId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseRate = table.Column<decimal>(type: "decimal(7,4)", nullable: true),
                    NetRate = table.Column<decimal>(type: "decimal(7,4)", nullable: true),
                    Premium = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Payroll = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Basis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exposure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserModifiedId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PolicyId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatingBases", x => x.RatingBasisId);
                    table.ForeignKey(
                        name: "FK_RatingBases_AspNetUsers_UserModifiedId",
                        column: x => x.UserModifiedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RatingBases_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RatingBases_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RatingBases_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkCompCoverages",
                columns: table => new
                {
                    WorkCompCoverageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EachAccident = table.Column<int>(type: "int", nullable: true),
                    DiseaseEachEmployee = table.Column<int>(type: "int", nullable: true),
                    DiseasePolicyLimit = table.Column<int>(type: "int", nullable: true),
                    OwnersOfficersExcluded = table.Column<bool>(type: "bit", nullable: true),
                    PerStatute = table.Column<bool>(type: "bit", nullable: true),
                    PerOther = table.Column<bool>(type: "bit", nullable: true),
                    WaiverOfSub = table.Column<bool>(type: "bit", nullable: true),
                    WaiverOfSubAttachmentAttachmentId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PolicyId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkCompCoverages", x => x.WorkCompCoverageId);
                    table.ForeignKey(
                        name: "FK_WorkCompCoverages_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkCompCoverages_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkCompCoverages_Attachments_WaiverOfSubAttachmentAttachmentId",
                        column: x => x.WaiverOfSubAttachmentAttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "AttachmentId");
                    table.ForeignKey(
                        name: "FK_WorkCompCoverages_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkCompCoverages_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutoCoverages_AdditionalAttachmentsAttachmentAttachmentId",
                table: "AutoCoverages",
                column: "AdditionalAttachmentsAttachmentAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoCoverages_AdditionalInsuredAttachmentAttachmentId",
                table: "AutoCoverages",
                column: "AdditionalInsuredAttachmentAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoCoverages_ClientId",
                table: "AutoCoverages",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoCoverages_CreatedById",
                table: "AutoCoverages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AutoCoverages_ModifiedById",
                table: "AutoCoverages",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_AutoCoverages_PolicyId",
                table: "AutoCoverages",
                column: "PolicyId",
                unique: true,
                filter: "[PolicyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AutoCoverages_WaiverOfSubAttachmentAttachmentId",
                table: "AutoCoverages",
                column: "WaiverOfSubAttachmentAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLiabilityCoverages_AdditionalAttachmentsAttachmentAttachmentId",
                table: "GeneralLiabilityCoverages",
                column: "AdditionalAttachmentsAttachmentAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLiabilityCoverages_AdditionalInsuredAttachmentAttachmentId",
                table: "GeneralLiabilityCoverages",
                column: "AdditionalInsuredAttachmentAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLiabilityCoverages_ClientId",
                table: "GeneralLiabilityCoverages",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLiabilityCoverages_CreatedById",
                table: "GeneralLiabilityCoverages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLiabilityCoverages_ModifiedById",
                table: "GeneralLiabilityCoverages",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLiabilityCoverages_PolicyId",
                table: "GeneralLiabilityCoverages",
                column: "PolicyId",
                unique: true,
                filter: "[PolicyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLiabilityCoverages_WaiverOfSubAttachmentAttachmentId",
                table: "GeneralLiabilityCoverages",
                column: "WaiverOfSubAttachmentAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Losses_PolicyId",
                table: "Losses",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Losses_UserModifiedId",
                table: "Losses",
                column: "UserModifiedId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingBases_LocationId",
                table: "RatingBases",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingBases_PolicyId",
                table: "RatingBases",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingBases_ProductId",
                table: "RatingBases",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RatingBases_UserModifiedId",
                table: "RatingBases",
                column: "UserModifiedId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkCompCoverages_ClientId",
                table: "WorkCompCoverages",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkCompCoverages_CreatedById",
                table: "WorkCompCoverages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkCompCoverages_ModifiedById",
                table: "WorkCompCoverages",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkCompCoverages_PolicyId",
                table: "WorkCompCoverages",
                column: "PolicyId",
                unique: true,
                filter: "[PolicyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WorkCompCoverages_WaiverOfSubAttachmentAttachmentId",
                table: "WorkCompCoverages",
                column: "WaiverOfSubAttachmentAttachmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutoCoverages");

            migrationBuilder.DropTable(
                name: "GeneralLiabilityCoverages");

            migrationBuilder.DropTable(
                name: "Losses");

            migrationBuilder.DropTable(
                name: "RatingBases");

            migrationBuilder.DropTable(
                name: "WorkCompCoverages");

            migrationBuilder.DropColumn(
                name: "BuildingTotalSquareFootage",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "GrossSales",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "NumFullTimeEmployees",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "NumPartTimeEmployees",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "NumStories",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "OccupiedSquareFootage",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Owner",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Tenant",
                table: "Locations");
        }
    }
}
