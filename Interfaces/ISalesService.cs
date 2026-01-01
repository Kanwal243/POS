using EyeHospitalPOS.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Interfaces
{
    public interface ISalesService
    {
        Task<Sale> CreateSaleAsync(Sale sale);
        Task<List<Sale>> GetSalesByDateAsync(DateTime date);
        Task<List<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<Sale>> GetSalesByUserAsync(string userId);
        Task<decimal> GetTodaysTotalSalesAsync();
        Task<int> GetTodaysTotalOrdersAsync();
        Task<decimal> GetTotalSalesAmountAsync();
        Task<List<KeyValuePair<string, decimal>>> GetTopCustomersAsync(int count);
        Task<List<KeyValuePair<string, decimal>>> GetTopProductsAsync(int count);
        Task<List<KeyValuePair<string, decimal>>> GetSalesByCategoryAsync();
        Task<Sale?> GetSaleByIdAsync(int id);
        string GenerateInvoiceNumber();
    }
}
