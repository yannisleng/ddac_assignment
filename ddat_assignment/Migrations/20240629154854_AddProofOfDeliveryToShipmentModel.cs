using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ddat_assignment.Migrations
{
    /// <inheritdoc />
    public partial class AddProofOfDeliveryToShipmentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProofOfDeliveryContentType",
                table: "ShipmentModel",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProofOfDeliveryFileName",
                table: "ShipmentModel",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProofOfDeliveryContentType",
                table: "ShipmentModel");

            migrationBuilder.DropColumn(
                name: "ProofOfDeliveryFileName",
                table: "ShipmentModel");
        }
    }
}
