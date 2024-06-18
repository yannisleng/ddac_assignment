using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ddat_assignment.Migrations
{
    /// <inheritdoc />
    public partial class addUserDetailsModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDetailsModel",
                columns: table => new
                {
                    UserDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserId1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetailsModel", x => x.UserDetailsId);
                    table.ForeignKey(
                        name: "FK_UserDetailsModel_AspNetUsers_UserId1",
                        column: x => x.UserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDetailsModel_UserId1",
                table: "UserDetailsModel",
                column: "UserId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDetailsModel");
        }
    }
}
