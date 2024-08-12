using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class SubmissionPremiumUnderwriterContacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Premium",
                table: "Submissions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusInt",
                table: "Submissions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewSubmissionEmail",
                table: "Carriers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServicingEmail",
                table: "Carriers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServicingWebsite",
                table: "Carriers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Premium",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "StatusInt",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "NewSubmissionEmail",
                table: "Carriers");

            migrationBuilder.DropColumn(
                name: "ServicingEmail",
                table: "Carriers");

            migrationBuilder.DropColumn(
                name: "ServicingWebsite",
                table: "Carriers");
        }
    }
}
