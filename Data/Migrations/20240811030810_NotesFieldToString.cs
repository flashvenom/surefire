using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class NotesFieldToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Submissions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Submissions");
        }
    }
}
