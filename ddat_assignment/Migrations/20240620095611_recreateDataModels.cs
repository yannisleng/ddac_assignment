using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ddat_assignment.Migrations
{
    /// <inheritdoc />
    public partial class recreateDataModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParcelModel",
                columns: table => new
                {
                    ParcelId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParcelModel", x => x.ParcelId);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentSlotModel",
                columns: table => new
                {
                    ShipmentSlotId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    ShipmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SlotTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DriverId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentSlotModel", x => x.ShipmentSlotId);
                    table.ForeignKey(
                        name: "FK_ShipmentSlotModel_DriverModel_DriverId",
                        column: x => x.DriverId,
                        principalTable: "DriverModel",
                        principalColumn: "DriverId");
                });

            migrationBuilder.CreateTable(
                name: "ShipmentModel",
                columns: table => new
                {
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    DriverId = table.Column<int>(type: "int", nullable: true),
                    ParcelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SenderId = table.Column<int>(type: "int", nullable: true),
                    SenderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SenderPhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    SenderId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReceiverId = table.Column<int>(type: "int", nullable: true),
                    ReceiverName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReceiverPhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    ReceiverId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PickupAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ShipmentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShipmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProofOfDelivery = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    ShipmentSlotId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                        principalColumn: "DriverId");
                    table.ForeignKey(
                        name: "FK_ShipmentModel_ParcelModel_ParcelId",
                        column: x => x.ParcelId,
                        principalTable: "ParcelModel",
                        principalColumn: "ParcelId");
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
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
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
                    TransitionId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                name: "IX_ShipmentModel_ParcelId",
                table: "ShipmentModel",
                column: "ParcelId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentModel");

            migrationBuilder.DropTable(
                name: "TransitionModel");

            migrationBuilder.DropTable(
                name: "ShipmentModel");

            migrationBuilder.DropTable(
                name: "ParcelModel");

            migrationBuilder.DropTable(
                name: "ShipmentSlotModel");
        }
    }
}
