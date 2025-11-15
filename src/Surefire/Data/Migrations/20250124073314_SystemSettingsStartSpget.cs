using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Surefire.Migrations
{
    /// <inheritdoc />
    public partial class SystemSettingsStartSpget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    SettingsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpenAiApiKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DbType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DbConnectionString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayLinkStringTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileStore = table.Column<int>(type: "int", nullable: false),
                    AzureBlobConnectionString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AzureBlobContainerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileServerMappedPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisablePlugins = table.Column<bool>(type: "bit", nullable: false)
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
