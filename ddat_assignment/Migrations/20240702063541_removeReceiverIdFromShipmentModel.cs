using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ddat_assignment.Migrations
{
    /// <inheritdoc />
    public partial class removeReceiverIdFromShipmentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentModel_AspNetUsers_ReceiverId",
                table: "ShipmentModel");

            migrationBuilder.DropIndex(
                name: "IX_ShipmentModel_ReceiverId",
                table: "ShipmentModel");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "ShipmentModel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiverId",
                table: "ShipmentModel",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentModel_ReceiverId",
                table: "ShipmentModel",
                column: "ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentModel_AspNetUsers_ReceiverId",
                table: "ShipmentModel",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
