using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ddat_assignment.Migrations
{
    /// <inheritdoc />
    public partial class updateShipmentSlotModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentModel_ShipmentSlotModel_ShipmentSlotId",
                table: "ShipmentModel");

            migrationBuilder.RenameColumn(
                name: "ShipmentSlotId",
                table: "ShipmentModel",
                newName: "ShipmentSlotModelShipmentSlotId");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentModel_ShipmentSlotId",
                table: "ShipmentModel",
                newName: "IX_ShipmentModel_ShipmentSlotModelShipmentSlotId");

            migrationBuilder.AddColumn<string>(
                name: "ShipmentIds",
                table: "ShipmentSlotModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentModel_ShipmentSlotModel_ShipmentSlotModelShipmentSlotId",
                table: "ShipmentModel",
                column: "ShipmentSlotModelShipmentSlotId",
                principalTable: "ShipmentSlotModel",
                principalColumn: "ShipmentSlotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentModel_ShipmentSlotModel_ShipmentSlotModelShipmentSlotId",
                table: "ShipmentModel");

            migrationBuilder.DropColumn(
                name: "ShipmentIds",
                table: "ShipmentSlotModel");

            migrationBuilder.RenameColumn(
                name: "ShipmentSlotModelShipmentSlotId",
                table: "ShipmentModel",
                newName: "ShipmentSlotId");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentModel_ShipmentSlotModelShipmentSlotId",
                table: "ShipmentModel",
                newName: "IX_ShipmentModel_ShipmentSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentModel_ShipmentSlotModel_ShipmentSlotId",
                table: "ShipmentModel",
                column: "ShipmentSlotId",
                principalTable: "ShipmentSlotModel",
                principalColumn: "ShipmentSlotId");
        }
    }
}
