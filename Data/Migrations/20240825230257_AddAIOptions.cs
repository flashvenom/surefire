using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class AddAIOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalInsuredFormNumber",
                table: "GeneralLiabilityCoverages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PrimaryWording",
                table: "GeneralLiabilityCoverages",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalInsuredFormNumber",
                table: "GeneralLiabilityCoverages");

            migrationBuilder.DropColumn(
                name: "PrimaryWording",
                table: "GeneralLiabilityCoverages");
        }
    }
}
