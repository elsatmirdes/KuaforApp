using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuaforApp.Migrations
{
    /// <inheritdoc />
    public partial class employeeAddUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "userID",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userID",
                table: "Employees");
        }
    }
}
