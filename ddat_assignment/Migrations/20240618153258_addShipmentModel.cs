using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ddat_assignment.Migrations
{
    /// <inheritdoc />
    public partial class addShipmentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShipmentModel",
                columns: table => new
                {
                    ShipmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverId = table.Column<int>(type: "int", nullable: false),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    SenderId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReceiverId = table.Column<int>(type: "int", nullable: false),
                    ReceiverId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PickupAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ShipmentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShipmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProofOfDelivery = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    ShipmentSlotId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentModel", x => x.ShipmentId);
                    table.ForeignKey(
                        name: "FK_ShipmentModel_AspNetUsers_ReceiverId1",
                        column: x => x.ReceiverId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShipmentModel_AspNetUsers_SenderId1",
                        column: x => x.SenderId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShipmentModel_DriverModel_DriverId",
                        column: x => x.DriverId,
                        principalTable: "DriverModel",
                        principalColumn: "DriverId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipmentModel_ShipmentSlotModel_ShipmentSlotId",
                        column: x => x.ShipmentSlotId,
                        principalTable: "ShipmentSlotModel",
                        principalColumn: "ShipmentSlotId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentModel_DriverId",
                table: "ShipmentModel",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentModel_ReceiverId1",
                table: "ShipmentModel",
                column: "ReceiverId1");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentModel_SenderId1",
                table: "ShipmentModel",
                column: "SenderId1");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentModel_ShipmentSlotId",
                table: "ShipmentModel",
                column: "ShipmentSlotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipmentModel");
        }
    }
}
