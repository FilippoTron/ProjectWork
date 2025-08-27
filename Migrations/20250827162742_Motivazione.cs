using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectWork.Migrations
{
    /// <inheritdoc />
    public partial class Motivazione : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Motivazione",
                table: "LoanRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Motivazione",
                table: "LoanRequests");
        }
    }
}
