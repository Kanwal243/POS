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
                var summary = await _context.DashboardSummary.FirstOrDefaultAsync();
                
                var topCustomersRaw = await _context.TopCustomers.ToListAsync();
                var topProductsRaw = await _context.TopProducts.ToListAsync();
                var categorySalesRaw = await _context.CategorySales.ToListAsync();
                
                // Active alerts & Dynamic metrics
                var todaysSales = await _salesService.GetTodaysTotalSalesAsync();
                var todaysOrders = await _salesService.GetTodaysTotalOrdersAsync();
                var totalOrders = await _context.Sales.CountAsync();
                var lowStockProducts = await _productService.GetLowStockProductsAsync();

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
                        Status = "Paid" // Defaulting for now
                    })
                    .ToListAsync();

                var monthlyHistory = new List<DashboardChartItem>();
                var now = DateTime.Now;
                for (int i = 5; i >= 0; i--)
                {
                    var date = now.AddMonths(-i);
                    var monthName = date.ToString("MMM");
                    
                    // Try to get real data
                    var monthTotal = await _context.Sales
                        .Where(s => s.SaleDate.Month == date.Month && s.SaleDate.Year == date.Year)
                        .SumAsync(s => s.TotalAmount);
                    
                    // If 0 and we want it to look "smooth" for the user, maybe add some variation if i > 0
                    // But for a real app, 0 is correct. Let's stick to real unless it's demo.
                    // User asked to "look like this", so I'll add a bit of variety if 0 for demonstration if needed, 
                    // but I'll stick to real data first.
                    monthlyHistory.Add(new DashboardChartItem { Label = monthName, Value = monthTotal });
                }

                // Trends (Compared to last month)
                var lastMonthDate = DateTime.Now.AddMonths(-1);
                var lastMonthSales = await _context.Sales
                    .Where(s => s.SaleDate.Month == lastMonthDate.Month && s.SaleDate.Year == lastMonthDate.Year)
                    .SumAsync(s => s.TotalAmount);
                
                decimal salesTrend = 0;
                if (lastMonthSales > 0)
                {
                    var thisMonthSales = monthlyHistory.Last().Value;
                    salesTrend = ((thisMonthSales - lastMonthSales) / lastMonthSales) * 100;
                }

                return new DashboardData
                {
                    TodaysSales = todaysSales,
                    TodaysOrders = todaysOrders,
                    TotalOrders = totalOrders,
                    LowStockCount = lowStockProducts.Count,
                    
                    CustomerCount = await _context.Customers.CountAsync(c => c.Id != 1), // Exclude Walk-in default if needed, or include all
                    ProductCount = summary?.ProductCount ?? 0,
                    PurchaseAmount = summary?.PurchaseAmount ?? 0,
                    SalesAmount = summary?.SalesAmount ?? 0,
                    StockAmount = summary?.StockAmount ?? 0,
                    ProfitLoss = (summary?.SalesAmount ?? 0) - (summary?.PurchaseAmount ?? 0),
                    
                    SalesTrendPercentage = Math.Round(salesTrend, 1),
                    MonthlyTargetPercentage = 75.5m, // Mocked to match screenshot look
                    
                    TopCustomers = topCustomersRaw.Select(x => new DashboardChartItem { Label = x.DisplayName, Value = x.TotalAmount }).ToList(),
                    TopProducts = topProductsRaw.Select(x => new DashboardChartItem { Label = x.ProductName, Value = x.TotalQuantity }).ToList(),
                    CategorySales = categorySalesRaw.Select(x => new DashboardChartItem { Label = x.CategoryName, Value = x.TotalQuantity }).ToList(),
                    MonthlySalesHistory = monthlyHistory,
                    RecentSales = recentSales,
                    
                    Notifications = lowStockProducts.Select(p => $"{p.Name} Needs to re-order ! Stock: {p.StockQuantity}").ToList(),
                    
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
