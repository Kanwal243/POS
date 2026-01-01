using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EyeHospitalPOS.Migrations
{
    /// <inheritdoc />
    public partial class AddShowInSelectionListToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowInSelectionList",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Address", "Email", "FullName", "IsActive", "Phone", "ShowInSelectionList" },
                values: new object[] { 1, "", "", "Cash / Walk-in", true, "0000000000", true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "ShowInSelectionList",
                table: "Customers");
        }
    }
}
