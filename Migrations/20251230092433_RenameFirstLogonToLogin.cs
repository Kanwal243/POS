using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EyeHospitalPOS.Migrations
{
    /// <inheritdoc />
    public partial class RenameFirstLogonToLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChangePasswordOnFirstLogon",
                table: "Users",
                newName: "ChangePasswordOnLogin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChangePasswordOnLogin",
                table: "Users",
                newName: "ChangePasswordOnFirstLogon");
        }
    }
}
