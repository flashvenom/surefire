using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class RenewalLastDataUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastRenewalId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastRenewalMonth",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastRenewalPerson",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastRenewalYear",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRenewalId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastRenewalMonth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastRenewalPerson",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastRenewalYear",
                table: "AspNetUsers");
        }
    }
}
