using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EyeHospitalPOS.Migrations
{
    /// <inheritdoc />
    public partial class FixDashboardViewSql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER VIEW View_DashboardSummary AS
                SELECT 
                    (SELECT COUNT(*) FROM Customers) AS CustomerCount,
                    (SELECT COUNT(*) FROM Products WHERE IsActive = 1) AS ProductCount,
                    ISNULL((SELECT SUM(TotalAmount) FROM PurchaseOrders WHERE Status IN (1, 2)), 0) AS PurchaseAmount,
                    ISNULL((SELECT SUM(TotalAmount) FROM Sales), 0) AS SalesAmount,
                    ISNULL((SELECT SUM(StockQuantity * SalePrice) FROM Products WHERE IsActive = 1), 0) AS StockAmount
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No safe revert possible as the previous column 'ApprovalStatus' no longer exists
        }
    }
}
