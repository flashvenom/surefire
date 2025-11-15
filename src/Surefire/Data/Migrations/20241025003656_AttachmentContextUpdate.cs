using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Surefire.Migrations
{
    /// <inheritdoc />
    public partial class AttachmentContextUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AspNetUsers_UploadedById",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AttachmentGroup_AttachmentGroupId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Carriers_CarrierId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Clients_ClientId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Folder_FolderId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Policies_PolicyId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Renewals_RenewalId",
                table: "Attachments");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AspNetUsers_UploadedById",
                table: "Attachments",
                column: "UploadedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AttachmentGroup_AttachmentGroupId",
                table: "Attachments",
                column: "AttachmentGroupId",
                principalTable: "AttachmentGroup",
                principalColumn: "AttachmentGroupId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Carriers_CarrierId",
                table: "Attachments",
                column: "CarrierId",
                principalTable: "Carriers",
                principalColumn: "CarrierId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Clients_ClientId",
                table: "Attachments",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Folder_FolderId",
                table: "Attachments",
                column: "FolderId",
                principalTable: "Folder",
                principalColumn: "FolderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Policies_PolicyId",
                table: "Attachments",
                column: "PolicyId",
                principalTable: "Policies",
                principalColumn: "PolicyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Renewals_RenewalId",
                table: "Attachments",
                column: "RenewalId",
                principalTable: "Renewals",
                principalColumn: "RenewalId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AspNetUsers_UploadedById",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AttachmentGroup_AttachmentGroupId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Carriers_CarrierId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Clients_ClientId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Folder_FolderId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Policies_PolicyId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Renewals_RenewalId",
                table: "Attachments");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AspNetUsers_UploadedById",
                table: "Attachments",
                column: "UploadedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AttachmentGroup_AttachmentGroupId",
                table: "Attachments",
                column: "AttachmentGroupId",
                principalTable: "AttachmentGroup",
                principalColumn: "AttachmentGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Carriers_CarrierId",
                table: "Attachments",
                column: "CarrierId",
                principalTable: "Carriers",
                principalColumn: "CarrierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Clients_ClientId",
                table: "Attachments",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Folder_FolderId",
                table: "Attachments",
                column: "FolderId",
                principalTable: "Folder",
                principalColumn: "FolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Policies_PolicyId",
                table: "Attachments",
                column: "PolicyId",
                principalTable: "Policies",
                principalColumn: "PolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Renewals_RenewalId",
                table: "Attachments",
                column: "RenewalId",
                principalTable: "Renewals",
                principalColumn: "RenewalId");
        }
    }
}
