using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ddat_assignment.Migrations
{
    /// <inheritdoc />
    public partial class addTransitionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_TransitionModel_ShipmentId",
                table: "TransitionModel",
                column: "ShipmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransitionModel");
        }
    }
}
