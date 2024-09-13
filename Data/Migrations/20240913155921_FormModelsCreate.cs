using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class FormModelsCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormPdf",
                columns: table => new
                {
                    FormPdfId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Filepath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JSONFields = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormPdf", x => x.FormPdfId);
                    table.ForeignKey(
                        name: "FK_FormPdf_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormPdf_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormDocs",
                columns: table => new
                {
                    FormDocId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JSONData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FormPdfId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormDocs", x => x.FormDocId);
                    table.ForeignKey(
                        name: "FK_FormDocs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormDocs_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormDocs_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormDocs_FormPdf_FormPdfId",
                        column: x => x.FormPdfId,
                        principalTable: "FormPdf",
                        principalColumn: "FormPdfId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormDocRevisions",
                columns: table => new
                {
                    FormDocRevisionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RevisionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JSONData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FormDocId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormDocRevisions", x => x.FormDocRevisionId);
                    table.ForeignKey(
                        name: "FK_FormDocRevisions_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormDocRevisions_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormDocRevisions_FormDocs_FormDocId",
                        column: x => x.FormDocId,
                        principalTable: "FormDocs",
                        principalColumn: "FormDocId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormDocRevisions_CreatedById",
                table: "FormDocRevisions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FormDocRevisions_FormDocId",
                table: "FormDocRevisions",
                column: "FormDocId");

            migrationBuilder.CreateIndex(
                name: "IX_FormDocRevisions_ModifiedById",
                table: "FormDocRevisions",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_FormDocs_ClientId",
                table: "FormDocs",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_FormDocs_CreatedById",
                table: "FormDocs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FormDocs_FormPdfId",
                table: "FormDocs",
                column: "FormPdfId");

            migrationBuilder.CreateIndex(
                name: "IX_FormDocs_ModifiedById",
                table: "FormDocs",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_FormPdf_CreatedById",
                table: "FormPdf",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FormPdf_ModifiedById",
                table: "FormPdf",
                column: "ModifiedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormDocRevisions");

            migrationBuilder.DropTable(
                name: "FormDocs");

            migrationBuilder.DropTable(
                name: "FormPdf");
        }
    }
}
