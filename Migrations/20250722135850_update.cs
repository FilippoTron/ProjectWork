using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectWork.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "LoanRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "LoanRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequests_UserId",
                table: "LoanRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanRequests_Users_UserId",
                table: "LoanRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanRequests_Users_UserId",
                table: "LoanRequests");

            migrationBuilder.DropIndex(
                name: "IX_LoanRequests_UserId",
                table: "LoanRequests");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "LoanRequests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LoanRequests");
        }
    }
}
