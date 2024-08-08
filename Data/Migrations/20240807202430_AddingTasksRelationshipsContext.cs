using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class AddingTasksRelationshipsContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackTask_AspNetUsers_AssignedToId",
                table: "TrackTask");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackTask_Renewals_RenewalId",
                table: "TrackTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrackTask",
                table: "TrackTask");

            migrationBuilder.RenameTable(
                name: "TrackTask",
                newName: "TrackTasks");

            migrationBuilder.RenameIndex(
                name: "IX_TrackTask_RenewalId",
                table: "TrackTasks",
                newName: "IX_TrackTasks_RenewalId");

            migrationBuilder.RenameIndex(
                name: "IX_TrackTask_AssignedToId",
                table: "TrackTasks",
                newName: "IX_TrackTasks_AssignedToId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrackTasks",
                table: "TrackTasks",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TaskMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<int>(type: "int", nullable: false),
                    Important = table.Column<bool>(type: "bit", nullable: false),
                    ParentTaskId = table.Column<int>(type: "int", nullable: true),
                    TaskName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DaysBeforeExpiration = table.Column<int>(type: "int", nullable: true),
                    ForType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskMasters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskMasters_TaskMasters_ParentTaskId",
                        column: x => x.ParentTaskId,
                        principalTable: "TaskMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskMasters_ParentTaskId",
                table: "TaskMasters",
                column: "ParentTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackTasks_AspNetUsers_AssignedToId",
                table: "TrackTasks",
                column: "AssignedToId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackTasks_Renewals_RenewalId",
                table: "TrackTasks",
                column: "RenewalId",
                principalTable: "Renewals",
                principalColumn: "RenewalId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackTasks_AspNetUsers_AssignedToId",
                table: "TrackTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackTasks_Renewals_RenewalId",
                table: "TrackTasks");

            migrationBuilder.DropTable(
                name: "TaskMasters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrackTasks",
                table: "TrackTasks");

            migrationBuilder.RenameTable(
                name: "TrackTasks",
                newName: "TrackTask");

            migrationBuilder.RenameIndex(
                name: "IX_TrackTasks_RenewalId",
                table: "TrackTask",
                newName: "IX_TrackTask_RenewalId");

            migrationBuilder.RenameIndex(
                name: "IX_TrackTasks_AssignedToId",
                table: "TrackTask",
                newName: "IX_TrackTask_AssignedToId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrackTask",
                table: "TrackTask",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackTask_AspNetUsers_AssignedToId",
                table: "TrackTask",
                column: "AssignedToId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackTask_Renewals_RenewalId",
                table: "TrackTask",
                column: "RenewalId",
                principalTable: "Renewals",
                principalColumn: "RenewalId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
