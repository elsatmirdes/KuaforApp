using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuaforApp.Migrations
{
    /// <inheritdoc />
    public partial class randevuOnaySutunu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "acceptAppointment",
                table: "Appointments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "acceptAppointment",
                table: "Appointments");
        }
    }
}
