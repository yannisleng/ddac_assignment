using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ddat_assignment.Migrations
{
    /// <inheritdoc />
    public partial class addShipmentSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShipmentSlotModel",
                columns: table => new
                {
                    ShipmentSlotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SlotTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DriverId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentSlotModel", x => x.ShipmentSlotId);
                    table.ForeignKey(
                        name: "FK_ShipmentSlotModel_DriverModel_DriverId",
                        column: x => x.DriverId,
                        principalTable: "DriverModel",
                        principalColumn: "DriverId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentSlotModel_DriverId",
                table: "ShipmentSlotModel",
                column: "DriverId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipmentSlotModel");
        }
    }
}
