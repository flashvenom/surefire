using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Surefire.Migrations
{
    /// <inheritdoc />
    public partial class AttachmentForSubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubmissionId",
                table: "Attachments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_SubmissionId",
                table: "Attachments",
                column: "SubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Submissions_SubmissionId",
                table: "Attachments",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "SubmissionId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Submissions_SubmissionId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_SubmissionId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "SubmissionId",
                table: "Attachments");
        }
    }
}
