using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ddat_assignment.Migrations
{
    /// <inheritdoc />
    public partial class clearTableModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParcelModel");

            migrationBuilder.DropTable(
                name: "PaymentModel");

            migrationBuilder.DropTable(
                name: "TransitionModel");

            migrationBuilder.DropTable(
                name: "ShipmentModel");

            migrationBuilder.DropTable(
                name: "ShipmentSlotModel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParcelModel",
                columns: table => new
                {
                    ParcelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParcelModel", x => x.ParcelId);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentSlotModel",
                columns: table => new
                {
                    ShipmentSlotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverId = table.Column<int>(type: "int", nullable: false),
                    ShipmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SlotTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ShipmentModel",
                columns: table => new
                {
                    ShipmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverId = table.Column<int>(type: "int", nullable: false),
                    ReceiverId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SenderId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ShipmentSlotId = table.Column<int>(type: "int", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PickupAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ProofOfDelivery = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ReceiverId = table.Column<int>(type: "int", nullable: true),
                    ReceiverName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SenderId = table.Column<int>(type: "int", nullable: true),
                    SenderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ShipmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShipmentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
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

            migrationBuilder.CreateTable(
                name: "PaymentModel",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentModel", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_PaymentModel_ShipmentModel_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "ShipmentModel",
                        principalColumn: "ShipmentId");
                });

            migrationBuilder.CreateTable(
                name: "TransitionModel",
                columns: table => new
                {
                    TransitionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentId = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransitionModel", x => x.TransitionId);
                    table.ForeignKey(
                        name: "FK_TransitionModel_ShipmentModel_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "ShipmentModel",
                        principalColumn: "ShipmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentModel_ShipmentId",
                table: "PaymentModel",
                column: "ShipmentId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentSlotModel_DriverId",
                table: "ShipmentSlotModel",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_TransitionModel_ShipmentId",
                table: "TransitionModel",
                column: "ShipmentId");
        }
    }
}
