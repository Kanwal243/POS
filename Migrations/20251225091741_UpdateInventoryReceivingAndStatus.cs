using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EyeHospitalPOS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInventoryReceivingAndStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "InventoryReceivings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrderId",
                table: "InventoryReceivings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SupplierInvoiceDate",
                table: "InventoryReceivings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupplierInvoiceNo",
                table: "InventoryReceivings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReceivings_PurchaseOrderId",
                table: "InventoryReceivings",
                column: "PurchaseOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryReceivings_PurchaseOrders_PurchaseOrderId",
                table: "InventoryReceivings",
                column: "PurchaseOrderId",
                principalTable: "PurchaseOrders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryReceivings_PurchaseOrders_PurchaseOrderId",
                table: "InventoryReceivings");

            migrationBuilder.DropIndex(
                name: "IX_InventoryReceivings_PurchaseOrderId",
                table: "InventoryReceivings");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "InventoryReceivings");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderId",
                table: "InventoryReceivings");

            migrationBuilder.DropColumn(
                name: "SupplierInvoiceDate",
                table: "InventoryReceivings");

            migrationBuilder.DropColumn(
                name: "SupplierInvoiceNo",
                table: "InventoryReceivings");
        }
    }
}
