using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeSupportSystem.Migrations
{
    /// <inheritdoc />
    public partial class myinitialcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicketStatus",
                columns: table => new
                {
                },
                constraints: table =>
                {
                });
        }
    }
}
