using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationshipsForRenewals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Renewals_AspNetUsers_AssignedToId",
                table: "Renewals");

            migrationBuilder.DropForeignKey(
                name: "FK_Renewals_Policies_PolicyId",
                table: "Renewals");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Renewals_RenewalId",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackTask_Renewals_RenewalId",
                table: "TrackTask");

            migrationBuilder.AlterColumn<int>(
                name: "RenewalId",
                table: "TrackTask",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PolicyId",
                table: "Renewals",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AssignedToId",
                table: "Renewals",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Renewals_AspNetUsers_AssignedToId",
                table: "Renewals",
                column: "AssignedToId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Renewals_Policies_PolicyId",
                table: "Renewals",
                column: "PolicyId",
                principalTable: "Policies",
                principalColumn: "PolicyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Renewals_RenewalId",
                table: "Submissions",
                column: "RenewalId",
                principalTable: "Renewals",
                principalColumn: "RenewalId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackTask_Renewals_RenewalId",
                table: "TrackTask",
                column: "RenewalId",
                principalTable: "Renewals",
                principalColumn: "RenewalId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Renewals_AspNetUsers_AssignedToId",
                table: "Renewals");

            migrationBuilder.DropForeignKey(
                name: "FK_Renewals_Policies_PolicyId",
                table: "Renewals");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Renewals_RenewalId",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackTask_Renewals_RenewalId",
                table: "TrackTask");

            migrationBuilder.AlterColumn<int>(
                name: "RenewalId",
                table: "TrackTask",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PolicyId",
                table: "Renewals",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "AssignedToId",
                table: "Renewals",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Renewals_AspNetUsers_AssignedToId",
                table: "Renewals",
                column: "AssignedToId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Renewals_Policies_PolicyId",
                table: "Renewals",
                column: "PolicyId",
                principalTable: "Policies",
                principalColumn: "PolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Renewals_RenewalId",
                table: "Submissions",
                column: "RenewalId",
                principalTable: "Renewals",
                principalColumn: "RenewalId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackTask_Renewals_RenewalId",
                table: "TrackTask",
                column: "RenewalId",
                principalTable: "Renewals",
                principalColumn: "RenewalId");
        }
    }
}
