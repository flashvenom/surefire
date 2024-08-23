using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class AddLossRunEmailToCarrier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LossRunsEmail",
                table: "Carriers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LossRunsEmail",
                table: "Carriers");
        }
    }
}
