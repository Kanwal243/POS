using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EyeHospitalPOS.Migrations
{
    /// <inheritdoc />
    public partial class AddChangePasswordOnFirstLogon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ChangePasswordOnFirstLogon",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "1",
                column: "ChangePasswordOnFirstLogon",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangePasswordOnFirstLogon",
                table: "Users");
        }
    }
}
