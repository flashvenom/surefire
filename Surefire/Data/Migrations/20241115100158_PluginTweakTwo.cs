using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Surefire.Migrations
{
    /// <inheritdoc />
    public partial class PluginTweakTwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseAddress",
                table: "Plugins");

            migrationBuilder.RenameColumn(
                name: "TokenAddress",
                table: "Plugins",
                newName: "ShortDescription");

            migrationBuilder.RenameColumn(
                name: "PolicyClientsAddress",
                table: "Plugins",
                newName: "PluginWebsite");

            migrationBuilder.RenameColumn(
                name: "PoliciesAddress",
                table: "Plugins",
                newName: "HashId");

            migrationBuilder.RenameColumn(
                name: "ClientAddress",
                table: "Plugins",
                newName: "DeveloperName");

            migrationBuilder.AlterColumn<string>(
                name: "ClientSecret",
                table: "Plugins",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "Plugins",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BaseUri",
                table: "Plugins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrantType",
                table: "Plugins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RedirectUri",
                table: "Plugins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Scope",
                table: "Plugins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenUri",
                table: "Plugins",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseUri",
                table: "Plugins");

            migrationBuilder.DropColumn(
                name: "GrantType",
                table: "Plugins");

            migrationBuilder.DropColumn(
                name: "RedirectUri",
                table: "Plugins");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "Plugins");

            migrationBuilder.DropColumn(
                name: "TokenUri",
                table: "Plugins");

            migrationBuilder.RenameColumn(
                name: "ShortDescription",
                table: "Plugins",
                newName: "TokenAddress");

            migrationBuilder.RenameColumn(
                name: "PluginWebsite",
                table: "Plugins",
                newName: "PolicyClientsAddress");

            migrationBuilder.RenameColumn(
                name: "HashId",
                table: "Plugins",
                newName: "PoliciesAddress");

            migrationBuilder.RenameColumn(
                name: "DeveloperName",
                table: "Plugins",
                newName: "ClientAddress");

            migrationBuilder.AlterColumn<string>(
                name: "ClientSecret",
                table: "Plugins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "Plugins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BaseAddress",
                table: "Plugins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
