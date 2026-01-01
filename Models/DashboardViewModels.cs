using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EyeHospitalPOS.Models
{
    [Keyless]
    public class DashboardSummaryView
    {
        public int CustomerCount { get; set; }
        public int ProductCount { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal SalesAmount { get; set; }
        public decimal StockAmount { get; set; }
    }

    [Keyless]
    public class TopCustomerView
    {
        public string DisplayName { get; set; }
        public decimal TotalAmount { get; set; }
    }

    [Keyless]
    public class TopProductView
    {
        public string ProductName { get; set; }
        public decimal TotalQuantity { get; set; }
    }
    
    [Keyless]
    public class CategorySalesView
    {
        public string CategoryName { get; set; }
        public decimal TotalQuantity { get; set; }
    }

    public class DashboardChartItem
    {
        public string Label { get; set; }
        public decimal Value { get; set; }
    }
}
