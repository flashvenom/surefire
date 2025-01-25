using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Surefire.Data.MigrationsLocal
{
    /// <inheritdoc />
    public partial class AddSystemSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    SettingsId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OpenAiApiKey = table.Column<string>(type: "TEXT", nullable: true),
                    DbType = table.Column<string>(type: "TEXT", nullable: true),
                    DbConnectionString = table.Column<string>(type: "TEXT", nullable: true),
                    PayLinkStringTemplate = table.Column<string>(type: "TEXT", nullable: true),
                    FileStore = table.Column<int>(type: "INTEGER", nullable: false),
                    AzureBlobConnectionString = table.Column<string>(type: "TEXT", nullable: true),
                    AzureBlobContainerName = table.Column<string>(type: "TEXT", nullable: true),
                    FileServerMappedPath = table.Column<string>(type: "TEXT", nullable: true),
                    DisablePlugins = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.SettingsId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
