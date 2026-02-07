using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quickfire.Blazor.Migrations
{
    /// <inheritdoc />
    public partial class AddFormsLibraryAndCompanyManual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyManualAdminUserId",
                table: "Settings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FormPdfId",
                table: "FormDocs",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "FormsLibraryVersionId",
                table: "FormDocs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CompanyManualAuditEntries",
                columns: table => new
                {
                    CompanyManualAuditEntryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompanyManualPageId = table.Column<int>(type: "INTEGER", nullable: true),
                    CompanyManualRevisionId = table.Column<int>(type: "INTEGER", nullable: true),
                    CompanyManualSuggestionId = table.Column<int>(type: "INTEGER", nullable: true),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    DetailsJson = table.Column<string>(type: "TEXT", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ActorUserId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyManualAuditEntries", x => x.CompanyManualAuditEntryId);
                    table.ForeignKey(
                        name: "FK_CompanyManualAuditEntries_AspNetUsers_ActorUserId",
                        column: x => x.ActorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyManualPages",
                columns: table => new
                {
                    CompanyManualPageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", nullable: true),
                    ProcedureType = table.Column<string>(type: "TEXT", nullable: true),
                    LineOfBusiness = table.Column<string>(type: "TEXT", nullable: true),
                    SlaTarget = table.Column<string>(type: "TEXT", nullable: true),
                    OwnerUserId = table.Column<string>(type: "TEXT", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                    Keywords = table.Column<string>(type: "TEXT", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReviewBy = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReviewCycleDays = table.Column<int>(type: "INTEGER", nullable: true),
                    TaskGroupId = table.Column<int>(type: "INTEGER", nullable: true),
                    ParentPageId = table.Column<int>(type: "INTEGER", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedById = table.Column<string>(type: "TEXT", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ArchivedById = table.Column<string>(type: "TEXT", nullable: true),
                    PublishedRevisionId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyManualPages", x => x.CompanyManualPageId);
                    table.ForeignKey(
                        name: "FK_CompanyManualPages_AspNetUsers_ArchivedById",
                        column: x => x.ArchivedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyManualPages_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyManualPages_AspNetUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyManualPages_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyManualPages_CompanyManualPages_ParentPageId",
                        column: x => x.ParentPageId,
                        principalTable: "CompanyManualPages",
                        principalColumn: "CompanyManualPageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyManualPages_TaskGroups_TaskGroupId",
                        column: x => x.TaskGroupId,
                        principalTable: "TaskGroups",
                        principalColumn: "TaskGroupId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CompanyManualRevisions",
                columns: table => new
                {
                    CompanyManualRevisionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompanyManualPageId = table.Column<int>(type: "INTEGER", nullable: false),
                    RevisionNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    ContentHtml = table.Column<string>(type: "TEXT", nullable: false),
                    ContentText = table.Column<string>(type: "TEXT", nullable: false),
                    ChangeSummary = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PublishedById = table.Column<string>(type: "TEXT", nullable: true),
                    SourceSuggestionId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyManualRevisions", x => x.CompanyManualRevisionId);
                    table.ForeignKey(
                        name: "FK_CompanyManualRevisions_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyManualRevisions_AspNetUsers_PublishedById",
                        column: x => x.PublishedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyManualRevisions_CompanyManualPages_CompanyManualPageId",
                        column: x => x.CompanyManualPageId,
                        principalTable: "CompanyManualPages",
                        principalColumn: "CompanyManualPageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyManualSuggestions",
                columns: table => new
                {
                    CompanyManualSuggestionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompanyManualPageId = table.Column<int>(type: "INTEGER", nullable: false),
                    BasedOnRevisionId = table.Column<int>(type: "INTEGER", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    ContentHtml = table.Column<string>(type: "TEXT", nullable: false),
                    ContentText = table.Column<string>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SubmittedById = table.Column<string>(type: "TEXT", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReviewedById = table.Column<string>(type: "TEXT", nullable: true),
                    ReviewNotes = table.Column<string>(type: "TEXT", nullable: true),
                    OutcomeRevisionId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyManualSuggestions", x => x.CompanyManualSuggestionId);
                    table.ForeignKey(
                        name: "FK_CompanyManualSuggestions_AspNetUsers_ReviewedById",
                        column: x => x.ReviewedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyManualSuggestions_AspNetUsers_SubmittedById",
                        column: x => x.SubmittedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyManualSuggestions_CompanyManualPages_CompanyManualPageId",
                        column: x => x.CompanyManualPageId,
                        principalTable: "CompanyManualPages",
                        principalColumn: "CompanyManualPageId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyManualSuggestions_CompanyManualRevisions_BasedOnRevisionId",
                        column: x => x.BasedOnRevisionId,
                        principalTable: "CompanyManualRevisions",
                        principalColumn: "CompanyManualRevisionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyManualSuggestions_CompanyManualRevisions_OutcomeRevisionId",
                        column: x => x.OutcomeRevisionId,
                        principalTable: "CompanyManualRevisions",
                        principalColumn: "CompanyManualRevisionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormsLibrary",
                columns: table => new
                {
                    FormsLibraryEntryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    CarrierName = table.Column<string>(type: "TEXT", nullable: true),
                    WholesalerName = table.Column<string>(type: "TEXT", nullable: true),
                    MarketTag = table.Column<string>(type: "TEXT", nullable: true),
                    Rating = table.Column<int>(type: "INTEGER", nullable: true),
                    IsBookmarked = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateModified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: true),
                    ModifiedById = table.Column<string>(type: "TEXT", nullable: true),
                    ActiveVersionId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormsLibrary", x => x.FormsLibraryEntryId);
                    table.ForeignKey(
                        name: "FK_FormsLibrary_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormsLibrary_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormsLibraryVersions",
                columns: table => new
                {
                    FormsLibraryVersionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FormsLibraryEntryId = table.Column<int>(type: "INTEGER", nullable: false),
                    VersionNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    VersionLabel = table.Column<string>(type: "TEXT", nullable: true),
                    OriginalFileName = table.Column<string>(type: "TEXT", nullable: true),
                    StoredFileName = table.Column<string>(type: "TEXT", nullable: true),
                    RelativePath = table.Column<string>(type: "TEXT", nullable: true),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: true),
                    Pages = table.Column<int>(type: "INTEGER", nullable: true),
                    InteractiveElements = table.Column<int>(type: "INTEGER", nullable: true),
                    FormFieldCount = table.Column<int>(type: "INTEGER", nullable: true),
                    JsonFields = table.Column<string>(type: "TEXT", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UploadedById = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormsLibraryVersions", x => x.FormsLibraryVersionId);
                    table.ForeignKey(
                        name: "FK_FormsLibraryVersions_AspNetUsers_UploadedById",
                        column: x => x.UploadedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormsLibraryVersions_FormsLibrary_FormsLibraryEntryId",
                        column: x => x.FormsLibraryEntryId,
                        principalTable: "FormsLibrary",
                        principalColumn: "FormsLibraryEntryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormDocs_FormsLibraryVersionId",
                table: "FormDocs",
                column: "FormsLibraryVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualAuditEntries_ActorUserId",
                table: "CompanyManualAuditEntries",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualAuditEntries_CompanyManualPageId",
                table: "CompanyManualAuditEntries",
                column: "CompanyManualPageId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualAuditEntries_CompanyManualRevisionId",
                table: "CompanyManualAuditEntries",
                column: "CompanyManualRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualAuditEntries_CompanyManualSuggestionId",
                table: "CompanyManualAuditEntries",
                column: "CompanyManualSuggestionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualPages_ArchivedById",
                table: "CompanyManualPages",
                column: "ArchivedById");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualPages_CreatedById",
                table: "CompanyManualPages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualPages_OwnerUserId",
                table: "CompanyManualPages",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualPages_ParentPageId",
                table: "CompanyManualPages",
                column: "ParentPageId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualPages_PublishedRevisionId",
                table: "CompanyManualPages",
                column: "PublishedRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualPages_Slug",
                table: "CompanyManualPages",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualPages_TaskGroupId",
                table: "CompanyManualPages",
                column: "TaskGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualPages_UpdatedById",
                table: "CompanyManualPages",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualRevisions_CompanyManualPageId",
                table: "CompanyManualRevisions",
                column: "CompanyManualPageId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualRevisions_CreatedById",
                table: "CompanyManualRevisions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualRevisions_PublishedById",
                table: "CompanyManualRevisions",
                column: "PublishedById");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualRevisions_SourceSuggestionId",
                table: "CompanyManualRevisions",
                column: "SourceSuggestionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualSuggestions_BasedOnRevisionId",
                table: "CompanyManualSuggestions",
                column: "BasedOnRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualSuggestions_CompanyManualPageId",
                table: "CompanyManualSuggestions",
                column: "CompanyManualPageId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualSuggestions_OutcomeRevisionId",
                table: "CompanyManualSuggestions",
                column: "OutcomeRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualSuggestions_ReviewedById",
                table: "CompanyManualSuggestions",
                column: "ReviewedById");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualSuggestions_Status",
                table: "CompanyManualSuggestions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyManualSuggestions_SubmittedById",
                table: "CompanyManualSuggestions",
                column: "SubmittedById");

            migrationBuilder.CreateIndex(
                name: "IX_FormsLibrary_ActiveVersionId",
                table: "FormsLibrary",
                column: "ActiveVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_FormsLibrary_CarrierName",
                table: "FormsLibrary",
                column: "CarrierName");

            migrationBuilder.CreateIndex(
                name: "IX_FormsLibrary_CreatedById",
                table: "FormsLibrary",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FormsLibrary_IsBookmarked",
                table: "FormsLibrary",
                column: "IsBookmarked");

            migrationBuilder.CreateIndex(
                name: "IX_FormsLibrary_MarketTag",
                table: "FormsLibrary",
                column: "MarketTag");

            migrationBuilder.CreateIndex(
                name: "IX_FormsLibrary_ModifiedById",
                table: "FormsLibrary",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_FormsLibrary_Title",
                table: "FormsLibrary",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_FormsLibrary_WholesalerName",
                table: "FormsLibrary",
                column: "WholesalerName");

            migrationBuilder.CreateIndex(
                name: "IX_FormsLibraryVersions_FormsLibraryEntryId_VersionNumber",
                table: "FormsLibraryVersions",
                columns: new[] { "FormsLibraryEntryId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormsLibraryVersions_UploadedById",
                table: "FormsLibraryVersions",
                column: "UploadedById");

            migrationBuilder.AddForeignKey(
                name: "FK_FormDocs_FormsLibraryVersions_FormsLibraryVersionId",
                table: "FormDocs",
                column: "FormsLibraryVersionId",
                principalTable: "FormsLibraryVersions",
                principalColumn: "FormsLibraryVersionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyManualAuditEntries_CompanyManualPages_CompanyManualPageId",
                table: "CompanyManualAuditEntries",
                column: "CompanyManualPageId",
                principalTable: "CompanyManualPages",
                principalColumn: "CompanyManualPageId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyManualAuditEntries_CompanyManualRevisions_CompanyManualRevisionId",
                table: "CompanyManualAuditEntries",
                column: "CompanyManualRevisionId",
                principalTable: "CompanyManualRevisions",
                principalColumn: "CompanyManualRevisionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyManualAuditEntries_CompanyManualSuggestions_CompanyManualSuggestionId",
                table: "CompanyManualAuditEntries",
                column: "CompanyManualSuggestionId",
                principalTable: "CompanyManualSuggestions",
                principalColumn: "CompanyManualSuggestionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyManualPages_CompanyManualRevisions_PublishedRevisionId",
                table: "CompanyManualPages",
                column: "PublishedRevisionId",
                principalTable: "CompanyManualRevisions",
                principalColumn: "CompanyManualRevisionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyManualRevisions_CompanyManualSuggestions_SourceSuggestionId",
                table: "CompanyManualRevisions",
                column: "SourceSuggestionId",
                principalTable: "CompanyManualSuggestions",
                principalColumn: "CompanyManualSuggestionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FormsLibrary_FormsLibraryVersions_ActiveVersionId",
                table: "FormsLibrary",
                column: "ActiveVersionId",
                principalTable: "FormsLibraryVersions",
                principalColumn: "FormsLibraryVersionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormDocs_FormsLibraryVersions_FormsLibraryVersionId",
                table: "FormDocs");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyManualRevisions_CompanyManualPages_CompanyManualPageId",
                table: "CompanyManualRevisions");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyManualSuggestions_CompanyManualPages_CompanyManualPageId",
                table: "CompanyManualSuggestions");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyManualSuggestions_CompanyManualRevisions_BasedOnRevisionId",
                table: "CompanyManualSuggestions");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyManualSuggestions_CompanyManualRevisions_OutcomeRevisionId",
                table: "CompanyManualSuggestions");

            migrationBuilder.DropForeignKey(
                name: "FK_FormsLibrary_FormsLibraryVersions_ActiveVersionId",
                table: "FormsLibrary");

            migrationBuilder.DropTable(
                name: "CompanyManualAuditEntries");

            migrationBuilder.DropTable(
                name: "CompanyManualPages");

            migrationBuilder.DropTable(
                name: "CompanyManualRevisions");

            migrationBuilder.DropTable(
                name: "CompanyManualSuggestions");

            migrationBuilder.DropTable(
                name: "FormsLibraryVersions");

            migrationBuilder.DropTable(
                name: "FormsLibrary");

            migrationBuilder.DropIndex(
                name: "IX_FormDocs_FormsLibraryVersionId",
                table: "FormDocs");

            migrationBuilder.DropColumn(
                name: "CompanyManualAdminUserId",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "FormsLibraryVersionId",
                table: "FormDocs");

            migrationBuilder.AlterColumn<int>(
                name: "FormPdfId",
                table: "FormDocs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);
        }
    }
}
