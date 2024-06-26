using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ddat_assignment.Migrations
{
    /// <inheritdoc />
    public partial class updateShipmentModelUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentModel_AspNetUsers_ReceiverId1",
                table: "ShipmentModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentModel_AspNetUsers_SenderId1",
                table: "ShipmentModel");

            migrationBuilder.DropIndex(
                name: "IX_ShipmentModel_ReceiverId1",
                table: "ShipmentModel");

            migrationBuilder.DropIndex(
                name: "IX_ShipmentModel_SenderId1",
                table: "ShipmentModel");

            migrationBuilder.DropColumn(
                name: "ReceiverId1",
                table: "ShipmentModel");

            migrationBuilder.DropColumn(
                name: "SenderId1",
                table: "ShipmentModel");

            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                table: "ShipmentModel",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverId",
                table: "ShipmentModel",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentModel_ReceiverId",
                table: "ShipmentModel",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentModel_SenderId",
                table: "ShipmentModel",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentModel_AspNetUsers_ReceiverId",
                table: "ShipmentModel",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentModel_AspNetUsers_SenderId",
                table: "ShipmentModel",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentModel_AspNetUsers_ReceiverId",
                table: "ShipmentModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentModel_AspNetUsers_SenderId",
                table: "ShipmentModel");

            migrationBuilder.DropIndex(
                name: "IX_ShipmentModel_ReceiverId",
                table: "ShipmentModel");

            migrationBuilder.DropIndex(
                name: "IX_ShipmentModel_SenderId",
                table: "ShipmentModel");

            migrationBuilder.AlterColumn<int>(
                name: "SenderId",
                table: "ShipmentModel",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReceiverId",
                table: "ShipmentModel",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverId1",
                table: "ShipmentModel",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderId1",
                table: "ShipmentModel",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentModel_ReceiverId1",
                table: "ShipmentModel",
                column: "ReceiverId1");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentModel_SenderId1",
                table: "ShipmentModel",
                column: "SenderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentModel_AspNetUsers_ReceiverId1",
                table: "ShipmentModel",
                column: "ReceiverId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentModel_AspNetUsers_SenderId1",
                table: "ShipmentModel",
                column: "SenderId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
