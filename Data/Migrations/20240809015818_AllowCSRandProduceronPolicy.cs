using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class AllowCSRandProduceronPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CSRId",
                table: "Policies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Policies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProducerId",
                table: "Policies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Policies_CreatedById",
                table: "Policies",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_CSRId",
                table: "Policies",
                column: "CSRId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_ProducerId",
                table: "Policies",
                column: "ProducerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_AspNetUsers_CSRId",
                table: "Policies",
                column: "CSRId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_AspNetUsers_CreatedById",
                table: "Policies",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_AspNetUsers_ProducerId",
                table: "Policies",
                column: "ProducerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Policies_AspNetUsers_CSRId",
                table: "Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_Policies_AspNetUsers_CreatedById",
                table: "Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_Policies_AspNetUsers_ProducerId",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_CreatedById",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_CSRId",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_ProducerId",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "CSRId",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "ProducerId",
                table: "Policies");
        }
    }
}
