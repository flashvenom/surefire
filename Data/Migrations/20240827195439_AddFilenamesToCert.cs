using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class AddFilenamesToCert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AttachWOS",
                table: "Certificates",
                newName: "BlockAttachments");

            migrationBuilder.RenameColumn(
                name: "AttachAI",
                table: "Certificates",
                newName: "AttachWCWOS");

            migrationBuilder.AddColumn<bool>(
                name: "AttachGLAI",
                table: "Certificates",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachGLAIfilename",
                table: "Certificates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AttachGLWOS",
                table: "Certificates",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachGLWOSfilename",
                table: "Certificates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachWCWOSfilename",
                table: "Certificates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachGLAI",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "AttachGLAIfilename",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "AttachGLWOS",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "AttachGLWOSfilename",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "AttachWCWOSfilename",
                table: "Certificates");

            migrationBuilder.RenameColumn(
                name: "BlockAttachments",
                table: "Certificates",
                newName: "AttachWOS");

            migrationBuilder.RenameColumn(
                name: "AttachWCWOS",
                table: "Certificates",
                newName: "AttachAI");
        }
    }
}
