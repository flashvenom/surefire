using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class AttachmentFoldersAndGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AttachmentGroup_AttachmentGroupId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Folder_FolderId",
                table: "Attachments");

            migrationBuilder.DropTable(
                name: "AttachmentGroup");

            migrationBuilder.DropTable(
                name: "Folder");

            migrationBuilder.CreateTable(
                name: "AttachmentGroups",
                columns: table => new
                {
                    AttachmentGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentGroups", x => x.AttachmentGroupId);
                });

            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    FolderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.FolderId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AttachmentGroups_AttachmentGroupId",
                table: "Attachments",
                column: "AttachmentGroupId",
                principalTable: "AttachmentGroups",
                principalColumn: "AttachmentGroupId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Folders_FolderId",
                table: "Attachments",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "FolderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AttachmentGroups_AttachmentGroupId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Folders_FolderId",
                table: "Attachments");

            migrationBuilder.DropTable(
                name: "AttachmentGroups");

            migrationBuilder.DropTable(
                name: "Folders");

            migrationBuilder.CreateTable(
                name: "AttachmentGroup",
                columns: table => new
                {
                    AttachmentGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentGroup", x => x.AttachmentGroupId);
                });

            migrationBuilder.CreateTable(
                name: "Folder",
                columns: table => new
                {
                    FolderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folder", x => x.FolderId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AttachmentGroup_AttachmentGroupId",
                table: "Attachments",
                column: "AttachmentGroupId",
                principalTable: "AttachmentGroup",
                principalColumn: "AttachmentGroupId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Folder_FolderId",
                table: "Attachments",
                column: "FolderId",
                principalTable: "Folder",
                principalColumn: "FolderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
