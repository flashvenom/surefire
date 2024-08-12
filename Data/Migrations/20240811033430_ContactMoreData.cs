using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class ContactMoreData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Billing",
                table: "Contacts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInactive",
                table: "Contacts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Representative",
                table: "Contacts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Service",
                table: "Contacts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Underwriter",
                table: "Contacts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Billing",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "IsInactive",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Representative",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Service",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Underwriter",
                table: "Contacts");
        }
    }
}
