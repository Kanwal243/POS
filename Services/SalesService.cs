using EyeHospitalPOS.Data;
using EyeHospitalPOS.Models;
using EyeHospitalPOS.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Services
{
    public class SalesService : ISalesService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductService _productService;

        public SalesService(ApplicationDbContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        public async Task<Sale> CreateSaleAsync(Sale sale)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Generate invoice number
                sale.InvoiceNumber = GenerateInvoiceNumber();
                
                // Calculate totals
                sale.SubTotal = sale.SaleItems?.Sum(si => si.Subtotal) ?? 0;
                sale.TotalAmount = sale.SubTotal - sale.DiscountAmount;

                // Add sale
                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();

                // Create invoice
                var invoice = new Invoice
                {
                    InvoiceNumber = sale.InvoiceNumber,
                    SaleId = sale.Id,
                    InvoiceDate = DateTime.Now
                };
                _context.Invoices.Add(invoice);

                // Update stock for each item
                if (sale.SaleItems != null)
                {
                    foreach (var item in sale.SaleItems)
                    {
                        await _productService.UpdateStockAsync(item.ProductId, -item.Quantity);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return sale;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Sale>> GetSalesByDateAsync(DateTime date)
        {
            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.User)
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .Where(s => s.SaleDate.Date == date.Date)
                .ToListAsync();
        }

        public async Task<List<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.User)
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .Where(s => s.SaleDate.Date >= startDate.Date && s.SaleDate.Date <= endDate.Date)
                .ToListAsync();
        }

        public async Task<List<Sale>> GetSalesByUserAsync(string userId)
        {
            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .Where(s => s.UserId == userId)
                .ToListAsync();
        }

        public async Task<decimal> GetTodaysTotalSalesAsync()
        {
            var today = DateTime.Today;
            return await _context.Sales
                .Where(s => s.SaleDate.Date == today)
                .SumAsync(s => s.TotalAmount);
        }

        public async Task<int> GetTodaysTotalOrdersAsync()
        {
            var today = DateTime.Today;
            return await _context.Sales
                .CountAsync(s => s.SaleDate.Date == today);
        }

        public async Task<decimal> GetTotalSalesAmountAsync()
        {
            return await _context.Sales.SumAsync(s => s.TotalAmount);
        }

        public async Task<List<KeyValuePair<string, decimal>>> GetTopCustomersAsync(int count)
        {
            var data = await _context.Sales
                .Where(s => s.Customer != null)
                .GroupBy(s => s.Customer.DisplayName)
                .Select(g => new { Label = g.Key, Value = g.Sum(s => s.TotalAmount) })
                .OrderByDescending(x => x.Value)
                .Take(count)
                .ToListAsync();

            return data.Select(x => new KeyValuePair<string, decimal>(x.Label, x.Value)).ToList();
        }

        public async Task<List<KeyValuePair<string, decimal>>> GetTopProductsAsync(int count)
        {
            var data = await _context.SaleItems
                .Where(si => si.Product != null)
                .GroupBy(si => si.Product.Name)
                .Select(g => new { Label = g.Key, Value = (decimal)g.Sum(si => si.Quantity) })
                .OrderByDescending(x => x.Value)
                .Take(count)
                .ToListAsync();

            return data.Select(x => new KeyValuePair<string, decimal>(x.Label, x.Value)).ToList();
        }

        public async Task<List<KeyValuePair<string, decimal>>> GetSalesByCategoryAsync()
        {
            var data = await _context.SaleItems
                .Where(si => si.Product != null && si.Product.Category != null)
                .GroupBy(si => si.Product.Category.Name)
                .Select(g => new { Label = g.Key, Value = (decimal)g.Sum(si => si.Quantity) })
                .ToListAsync();

            return data.Select(x => new KeyValuePair<string, decimal>(x.Label, x.Value)).ToList();
        }

        public async Task<Sale?> GetSaleByIdAsync(int id)
        {
            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.User)
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .Include(s => s.Invoice)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public string GenerateInvoiceNumber()
        {
            var today = DateTime.Today;
            var prefix = $"INV{today:yyyyMMdd}";
            
            var lastInvoice = _context.Invoices
                .Where(i => i.InvoiceNumber.StartsWith(prefix))
                .OrderByDescending(i => i.InvoiceNumber)
                .FirstOrDefault();

            if (lastInvoice == null)
            {
                return $"{prefix}001";
            }

            var lastNumber = int.Parse(lastInvoice.InvoiceNumber.Substring(prefix.Length));
            return $"{prefix}{(lastNumber + 1):D3}";
        }
    }
}
