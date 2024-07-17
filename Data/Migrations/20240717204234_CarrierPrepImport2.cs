using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mantis.Migrations
{
    /// <inheritdoc />
    public partial class CarrierPrepImport2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carriers_Address_AddressId",
                table: "Carriers");

            migrationBuilder.AlterColumn<int>(
                name: "AddressId",
                table: "Carriers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Carriers_Address_AddressId",
                table: "Carriers",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "AddressId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carriers_Address_AddressId",
                table: "Carriers");

            migrationBuilder.AlterColumn<int>(
                name: "AddressId",
                table: "Carriers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Carriers_Address_AddressId",
                table: "Carriers",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
