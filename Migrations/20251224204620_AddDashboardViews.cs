using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EyeHospitalPOS.Migrations
{
    /// <inheritdoc />
    public partial class AddDashboardViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VIEW View_DashboardSummary AS
                SELECT 
                    (SELECT COUNT(*) FROM Customers) AS CustomerCount,
                    (SELECT COUNT(*) FROM Products WHERE IsActive = 1) AS ProductCount,
                    ISNULL((SELECT SUM(TotalAmount) FROM PurchaseOrders WHERE ApprovalStatus = 1), 0) AS PurchaseAmount,
                    ISNULL((SELECT SUM(TotalAmount) FROM Sales), 0) AS SalesAmount,
                    ISNULL((SELECT SUM(StockQuantity * SalePrice) FROM Products WHERE IsActive = 1), 0) AS StockAmount
            ");

            migrationBuilder.Sql(@"
                CREATE VIEW View_TopCustomers AS
                SELECT TOP 5
                    c.DisplayName,
                    SUM(s.TotalAmount) AS TotalAmount
                FROM Customers c
                JOIN Sales s ON c.Id = s.CustomerId
                GROUP BY c.DisplayName
                ORDER BY TotalAmount DESC
            ");

            migrationBuilder.Sql(@"
                CREATE VIEW View_TopProducts AS
                SELECT TOP 5
                    p.Name AS ProductName,
                    SUM(si.Quantity) AS TotalQuantity
                FROM Products p
                JOIN SaleItems si ON p.Id = si.ProductId
                GROUP BY p.Name
                ORDER BY TotalQuantity DESC
            ");

            migrationBuilder.Sql(@"
                CREATE VIEW View_CategorySales AS
                SELECT 
                    pc.Name AS CategoryName,
                    SUM(si.Quantity) AS TotalQuantity
                FROM ProductCategories pc
                JOIN Products p ON pc.Id = p.CategoryId
                JOIN SaleItems si ON p.Id = si.ProductId
                GROUP BY pc.Name
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW View_DashboardSummary");
            migrationBuilder.Sql("DROP VIEW View_TopCustomers");
            migrationBuilder.Sql("DROP VIEW View_TopProducts");
            migrationBuilder.Sql("DROP VIEW View_CategorySales");
        }
    }
}
