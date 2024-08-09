using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteToTrackTaskRenewals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackTasks_Renewals_RenewalId",
                table: "TrackTasks");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackTasks_Renewals_RenewalId",
                table: "TrackTasks",
                column: "RenewalId",
                principalTable: "Renewals",
                principalColumn: "RenewalId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackTasks_Renewals_RenewalId",
                table: "TrackTasks");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackTasks_Renewals_RenewalId",
                table: "TrackTasks",
                column: "RenewalId",
                principalTable: "Renewals",
                principalColumn: "RenewalId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
