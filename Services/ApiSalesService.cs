using EyeHospitalPOS.Interfaces;
using EyeHospitalPOS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Services
{
    /// <summary>
    /// HttpClient-based API client for Sales/POS operations
    /// </summary>
    public class ApiSalesService : ApiClientService, ISalesService
    {
        public ApiSalesService(HttpClient httpClient, IJSRuntime jsRuntime, IHttpContextAccessor httpContextAccessor, Helper.LoginManager loginManager) 
            : base(httpClient, jsRuntime, httpContextAccessor, loginManager)
        {
        }

        public async Task<Sale> CreateSaleAsync(Sale sale)
        {
            return await PostAsync<Sale, Sale>("api/Sales", sale) 
                ?? throw new Exception("Failed to create sale");
        }

        public async Task<Sale?> GetSaleByIdAsync(int id)
        {
            return await GetAsync<Sale>($"api/Sales/{id}");
        }

        public async Task<List<Sale>> GetSalesByDateAsync(DateTime date)
        {
            return await GetAsync<List<Sale>>($"api/Sales/by-date?date={date:yyyy-MM-dd}") 
                ?? new List<Sale>();
        }

        public async Task<List<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await GetAsync<List<Sale>>($"api/Sales/range?start={startDate:yyyy-MM-dd}&end={endDate:yyyy-MM-dd}") 
                ?? new List<Sale>();
        }

        public async Task<List<Sale>> GetSalesByUserAsync(string userId)
        {
            return await GetAsync<List<Sale>>($"api/Sales/user/{userId}") 
                ?? new List<Sale>();
        }

        public async Task<decimal> GetTodaysTotalSalesAsync() => 
            await GetAsync<decimal>("api/Sales/today/total");

        public async Task<int> GetTodaysTotalOrdersAsync() => 
            await GetAsync<int>("api/Sales/today/count");

        public async Task<decimal> GetTotalSalesAmountAsync() => 
            await GetAsync<decimal>("api/Sales/total");

        public async Task<List<KeyValuePair<string, decimal>>> GetTopCustomersAsync(int count)
        {
            return await GetAsync<List<KeyValuePair<string, decimal>>>($"api/Sales/top-customers?count={count}") 
                ?? new List<KeyValuePair<string, decimal>>();
        }

        public async Task<List<KeyValuePair<string, decimal>>> GetTopProductsAsync(int count)
        {
            return await GetAsync<List<KeyValuePair<string, decimal>>>($"api/Sales/top-products?count={count}") 
                ?? new List<KeyValuePair<string, decimal>>();
        }

        public async Task<List<KeyValuePair<string, decimal>>> GetSalesByCategoryAsync()
        {
            return await GetAsync<List<KeyValuePair<string, decimal>>>("api/Sales/by-category") 
                ?? new List<KeyValuePair<string, decimal>>();
        }

        public string GenerateInvoiceNumber() => 
            $"INV-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
    }
}

