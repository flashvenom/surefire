using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class Logging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "FileData",
                table: "Attachments");

            migrationBuilder.RenameColumn(
                name: "StoreType",
                table: "Attachments",
                newName: "Comments");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Attachments",
                newName: "OriginalFileName");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AttachmentGroupId",
                table: "Attachments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarrierId",
                table: "Attachments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Attachments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateLastOpened",
                table: "Attachments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileFormat",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "Attachments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FolderId",
                table: "Attachments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HashedFileName",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsBinder",
                table: "Attachments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsClientAccessible",
                table: "Attachments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEndorsement",
                table: "Attachments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPolicyCopy",
                table: "Attachments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsProposal",
                table: "Attachments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsQuote",
                table: "Attachments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LocalPath",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Attachments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UploadedById",
                table: "Attachments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AttachmentGroup",
                columns: table => new
                {
                    AttachmentGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folder", x => x.FolderId);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogLevel = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_Logs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_AttachmentGroupId",
                table: "Attachments",
                column: "AttachmentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_CarrierId",
                table: "Attachments",
                column: "CarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_FolderId",
                table: "Attachments",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_UploadedById",
                table: "Attachments",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserId",
                table: "Logs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AspNetUsers_UploadedById",
                table: "Attachments",
                column: "UploadedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AttachmentGroup_AttachmentGroupId",
                table: "Attachments",
                column: "AttachmentGroupId",
                principalTable: "AttachmentGroup",
                principalColumn: "AttachmentGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Carriers_CarrierId",
                table: "Attachments",
                column: "CarrierId",
                principalTable: "Carriers",
                principalColumn: "CarrierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Folder_FolderId",
                table: "Attachments",
                column: "FolderId",
                principalTable: "Folder",
                principalColumn: "FolderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AspNetUsers_UploadedById",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AttachmentGroup_AttachmentGroupId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Carriers_CarrierId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Folder_FolderId",
                table: "Attachments");

            migrationBuilder.DropTable(
                name: "AttachmentGroup");

            migrationBuilder.DropTable(
                name: "Folder");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_AttachmentGroupId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_CarrierId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_FolderId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_UploadedById",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "AttachmentGroupId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "CarrierId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "DateLastOpened",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "FileFormat",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "FolderId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "HashedFileName",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "IsBinder",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "IsClientAccessible",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "IsEndorsement",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "IsPolicyCopy",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "IsProposal",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "IsQuote",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "LocalPath",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "UploadedById",
                table: "Attachments");

            migrationBuilder.RenameColumn(
                name: "OriginalFileName",
                table: "Attachments",
                newName: "FileName");

            migrationBuilder.RenameColumn(
                name: "Comments",
                table: "Attachments",
                newName: "StoreType");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FileData",
                table: "Attachments",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
