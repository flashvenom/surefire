using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class CertRemovePolicyDep4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Clients_ClientId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Policies_PolicyId",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_PolicyId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "PolicyId",
                table: "Certificates");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Clients_ClientId",
                table: "Certificates",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Clients_ClientId",
                table: "Certificates");

            migrationBuilder.AddColumn<int>(
                name: "PolicyId",
                table: "Certificates",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_PolicyId",
                table: "Certificates",
                column: "PolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Clients_ClientId",
                table: "Certificates",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Policies_PolicyId",
                table: "Certificates",
                column: "PolicyId",
                principalTable: "Policies",
                principalColumn: "PolicyId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
