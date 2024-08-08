using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class RenewalManagerPrep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedToId",
                table: "TrackTask",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "WholesalerId",
                table: "Renewals",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CarrierId",
                table: "Renewals",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "AssignedToId",
                table: "Renewals",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Renewals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TrackTask_AssignedToId",
                table: "TrackTask",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Renewals_AssignedToId",
                table: "Renewals",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Renewals_ClientId",
                table: "Renewals",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Renewals_AspNetUsers_AssignedToId",
                table: "Renewals",
                column: "AssignedToId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Renewals_Clients_ClientId",
                table: "Renewals",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackTask_AspNetUsers_AssignedToId",
                table: "TrackTask",
                column: "AssignedToId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Renewals_AspNetUsers_AssignedToId",
                table: "Renewals");

            migrationBuilder.DropForeignKey(
                name: "FK_Renewals_Clients_ClientId",
                table: "Renewals");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackTask_AspNetUsers_AssignedToId",
                table: "TrackTask");

            migrationBuilder.DropIndex(
                name: "IX_TrackTask_AssignedToId",
                table: "TrackTask");

            migrationBuilder.DropIndex(
                name: "IX_Renewals_AssignedToId",
                table: "Renewals");

            migrationBuilder.DropIndex(
                name: "IX_Renewals_ClientId",
                table: "Renewals");

            migrationBuilder.DropColumn(
                name: "AssignedToId",
                table: "TrackTask");

            migrationBuilder.DropColumn(
                name: "AssignedToId",
                table: "Renewals");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Renewals");

            migrationBuilder.AlterColumn<int>(
                name: "WholesalerId",
                table: "Renewals",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CarrierId",
                table: "Renewals",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
