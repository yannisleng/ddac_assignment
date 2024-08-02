using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ddat_assignment.Migrations
{
    /// <inheritdoc />
    public partial class AddOnlineTransferFieldsToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillingAddress",
                table: "PaymentModel",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProofOfPaymentPath",
                table: "PaymentModel",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingAddress",
                table: "PaymentModel");

            migrationBuilder.DropColumn(
                name: "ProofOfPaymentPath",
                table: "PaymentModel");
        }
    }
}
