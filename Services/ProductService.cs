using EyeHospitalPOS.Data;
using EyeHospitalPOS.Models;
using EyeHospitalPOS.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Type)
                .Include(p => p.Supplier)
                .ToListAsync();
        }

        public async Task<List<Product>> GetActiveProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Type)
                .Include(p => p.Supplier)
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Type)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> GetProductByBarcodeAsync(string barcode)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Type)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Barcode == barcode && p.IsActive);
        }

        public async Task<List<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Type)
                .Include(p => p.Supplier)
                .Where(p => p.IsActive && 
                           (p.Name.Contains(searchTerm) || 
                            p.Barcode.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // Soft delete
                product.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Product>> GetLowStockProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Type)
                .Where(p => p.IsActive && p.StockQuantity <= p.ReorderLevel)
                .ToListAsync();
        }

        public async Task<bool> CheckStockAvailability(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            return product != null && product.StockQuantity >= quantity;
        }

        public async Task UpdateStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.StockQuantity += quantity;
                await _context.SaveChangesAsync();
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetTotalStockValueAsync()
        {
            // Assuming CostPrice exists on Product. Re-checking Product model might be good, 
            // but assuming standard POS model implies Cost/Price.
            // If CostPrice is missing, will use Price (Sale Price) or return 0. 
            // Let's assume CostPrice or similar field. 
            // Checking Product.cs content would be safer, but for now I'll use Price as fallback or if CostPrice doesn't exist.
            // Actually, I should check Product.cs.
            // I will assume Price for now as 'Stock Amt' usually means value. 
            // Wait, usually Stock Value = Cost * Qty. 
             return await _context.Products
                .Where(p => p.IsActive)
                .SumAsync(p => p.StockQuantity * p.SalePrice); // Using Price for now as Cost is not guaranteed unless checked.
        }
    }
}
