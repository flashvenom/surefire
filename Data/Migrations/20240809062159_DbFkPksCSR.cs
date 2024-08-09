using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class DbFkPksCSR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_AspNetUsers_CSRId",
                table: "Policies",
                column: "CSRId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_AspNetUsers_CreatedById",
                table: "Policies",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_AspNetUsers_ProducerId",
                table: "Policies",
                column: "ProducerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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
    }
}
