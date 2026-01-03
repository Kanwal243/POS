using EyeHospitalPOS.Interfaces;
using EyeHospitalPOS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using EyeHospitalPOS.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Controllers
{
    public class DashboardController
    {
        private readonly ISalesService _salesService;
        private readonly IProductService _productService;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly EyeHospitalPOS.Data.ApplicationDbContext _context;

        public DashboardController(
            ISalesService salesService, 
            IProductService productService, 
            IPurchaseOrderService purchaseOrderService,
            EyeHospitalPOS.Data.ApplicationDbContext context)
        {
            _salesService = salesService;
            _productService = productService;
            _purchaseOrderService = purchaseOrderService;
            _context = context;
        }

        public async Task<DashboardData> LoadDashboardDataAsync()
        {
            try
            {
                // Basic metrics
                var todaysSales = await _salesService.GetTodaysTotalSalesAsync();
                var todaysOrders = await _salesService.GetTodaysTotalOrdersAsync();
                var totalOrders = await _context.Sales.CountAsync();
                var totalSalesAmount = await _salesService.GetTotalSalesAmountAsync();
                var customerCount = await _context.Customers.CountAsync();
                var productCount = await _context.Products.CountAsync(p => p.IsActive);
                var totalStockValue = await _productService.GetTotalStockValueAsync();
                
                // Purchase metrics (assuming simple sum for now if service doesn't have it, or using PO service if available)
                var purchaseAmount = await _context.PurchaseOrders.SumAsync(po => po.TotalAmount);

                // Inventory alerts
                var lowStockProducts = await _productService.GetLowStockProductsAsync();

                // Recent sales list
                var recentSales = await _context.Sales
                    .Include(s => s.Customer)
                    .OrderByDescending(s => s.SaleDate)
                    .Take(5)
                    .Select(s => new RecentSaleViewModel
                    {
                        InvoiceNumber = s.InvoiceNumber ?? $"INV-{s.Id}",
                        CustomerName = s.Customer != null ? s.Customer.DisplayName : "Walk-in",
                        Date = s.SaleDate,
                        Amount = s.TotalAmount,
                        Status = "Completed"
                    })
                    .ToListAsync();

                // Monthly History (Last 6 months)
                var monthlyHistory = new List<DashboardChartItem>();
                var now = DateTime.Now;
                for (int i = 5; i >= 0; i--)
                {
                    var date = now.AddMonths(-i);
                    var monthStart = new DateTime(date.Year, date.Month, 1);
                    var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                    
                    var monthTotal = await _context.Sales
                        .Where(s => s.SaleDate >= monthStart && s.SaleDate <= monthEnd)
                        .SumAsync(s => s.TotalAmount);
                    
                    monthlyHistory.Add(new DashboardChartItem 
                    { 
                        Label = date.ToString("MMM"), 
                        Value = monthTotal 
                    });
                }

                // Trends (Compared to last month)
                var lastMonthDate = DateTime.Now.AddMonths(-1);
                var lastMonthStart = new DateTime(lastMonthDate.Year, lastMonthDate.Month, 1);
                var lastMonthEnd = lastMonthStart.AddMonths(1).AddDays(-1);
                
                var lastMonthSales = await _context.Sales
                    .Where(s => s.SaleDate >= lastMonthStart && s.SaleDate <= lastMonthEnd)
                    .SumAsync(s => s.TotalAmount);
                
                decimal salesTrend = 0;
                if (lastMonthSales > 0)
                {
                    var thisMonthSales = monthlyHistory.Last().Value;
                    salesTrend = ((thisMonthSales - lastMonthSales) / lastMonthSales) * 100;
                }

                // Get chart data from service
                var topCustomers = await _salesService.GetTopCustomersAsync(5);
                var topProducts = await _salesService.GetTopProductsAsync(5);
                var categorySales = await _salesService.GetSalesByCategoryAsync();

                return new DashboardData
                {
                    TodaysSales = todaysSales,
                    TodaysOrders = todaysOrders,
                    TotalOrders = totalOrders,
                    LowStockCount = lowStockProducts.Count,
                    
                    CustomerCount = customerCount,
                    ProductCount = productCount,
                    PurchaseAmount = purchaseAmount,
                    SalesAmount = totalSalesAmount,
                    StockAmount = totalStockValue,
                    ProfitLoss = totalSalesAmount - purchaseAmount,
                    
                    SalesTrendPercentage = Math.Round(salesTrend, 1),
                    MonthlyTargetPercentage = 100, // Default target
                    
                    TopCustomers = topCustomers.Select(x => new DashboardChartItem { Label = x.Key, Value = x.Value }).ToList(),
                    TopProducts = topProducts.Select(x => new DashboardChartItem { Label = x.Key, Value = x.Value }).ToList(),
                    CategorySales = categorySales.Select(x => new DashboardChartItem { Label = x.Key, Value = x.Value }).ToList(),
                    MonthlySalesHistory = monthlyHistory,
                    RecentSales = recentSales,
                    
                    Notifications = lowStockProducts.Select(p => $"{p.Name} low on stock: {p.StockQuantity} (Reorder level: {p.ReorderLevel})").ToList(),
                    
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new DashboardData
                {
                    Success = false,
                    ErrorMessage = $"Error loading dashboard: {ex.Message}"
                };
            }
        }
    }

    public class RecentSaleViewModel
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class DashboardData
    {
        public decimal TodaysSales { get; set; }
        public int TodaysOrders { get; set; }
        public int TotalOrders { get; set; }
        public int LowStockCount { get; set; }
        
        public int CustomerCount { get; set; }
        public int ProductCount { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal SalesAmount { get; set; }
        public decimal StockAmount { get; set; }
        public decimal ProfitLoss { get; set; }
        
        public decimal SalesTrendPercentage { get; set; }
        public decimal MonthlyTargetPercentage { get; set; }
        
        public List<DashboardChartItem> TopCustomers { get; set; } = new();
        public List<DashboardChartItem> TopProducts { get; set; } = new();
        public List<DashboardChartItem> CategorySales { get; set; } = new();
        public List<DashboardChartItem> MonthlySalesHistory { get; set; } = new();
        public List<RecentSaleViewModel> RecentSales { get; set; } = new();
        public List<string> Notifications { get; set; } = new();
        
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
