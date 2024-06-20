using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ddat_assignment.Migrations
{
    /// <inheritdoc />
    public partial class updateDdat_assignmentUserAndDriverModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDetailsModel_AspNetUsers_UserId1",
                table: "UserDetailsModel");

            migrationBuilder.DropIndex(
                name: "IX_UserDetailsModel_UserId1",
                table: "UserDetailsModel");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserDetailsModel");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserDetailsModel",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "VehicleType",
                table: "DriverModel",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "VehiclePlateNumber",
                table: "DriverModel",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<DateTime>(
                name: "DrivingLicenseExpiryDate",
                table: "DriverModel",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DrivingLicenseType",
                table: "DriverModel",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactName",
                table: "DriverModel",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactPhone",
                table: "DriverModel",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactRelationship",
                table: "DriverModel",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredWorkingDay",
                table: "DriverModel",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredWorkingLocation",
                table: "DriverModel",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "DriverModel",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "AspNetUsers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDetailsModel_UserId",
                table: "UserDetailsModel",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDetailsModel_AspNetUsers_UserId",
                table: "UserDetailsModel",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDetailsModel_AspNetUsers_UserId",
                table: "UserDetailsModel");

            migrationBuilder.DropIndex(
                name: "IX_UserDetailsModel_UserId",
                table: "UserDetailsModel");

            migrationBuilder.DropColumn(
                name: "DrivingLicenseExpiryDate",
                table: "DriverModel");

            migrationBuilder.DropColumn(
                name: "DrivingLicenseType",
                table: "DriverModel");

            migrationBuilder.DropColumn(
                name: "EmergencyContactName",
                table: "DriverModel");

            migrationBuilder.DropColumn(
                name: "EmergencyContactPhone",
                table: "DriverModel");

            migrationBuilder.DropColumn(
                name: "EmergencyContactRelationship",
                table: "DriverModel");

            migrationBuilder.DropColumn(
                name: "PreferredWorkingDay",
                table: "DriverModel");

            migrationBuilder.DropColumn(
                name: "PreferredWorkingLocation",
                table: "DriverModel");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "DriverModel");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserDetailsModel",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "UserDetailsModel",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VehicleType",
                table: "DriverModel",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VehiclePlateNumber",
                table: "DriverModel",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDetailsModel_UserId1",
                table: "UserDetailsModel",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDetailsModel_AspNetUsers_UserId1",
                table: "UserDetailsModel",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
