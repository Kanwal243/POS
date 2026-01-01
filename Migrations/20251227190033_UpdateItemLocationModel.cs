using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EyeHospitalPOS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateItemLocationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ItemLocations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "ItemLocations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentLocationId",
                table: "ItemLocations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemLocations_ParentLocationId",
                table: "ItemLocations",
                column: "ParentLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemLocations_ItemLocations_ParentLocationId",
                table: "ItemLocations",
                column: "ParentLocationId",
                principalTable: "ItemLocations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemLocations_ItemLocations_ParentLocationId",
                table: "ItemLocations");

            migrationBuilder.DropIndex(
                name: "IX_ItemLocations_ParentLocationId",
                table: "ItemLocations");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "ItemLocations");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "ItemLocations");

            migrationBuilder.DropColumn(
                name: "ParentLocationId",
                table: "ItemLocations");
        }
    }
}
