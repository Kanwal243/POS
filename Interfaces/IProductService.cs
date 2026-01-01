using EyeHospitalPOS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<List<Product>> GetActiveProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product?> GetProductByBarcodeAsync(string barcode);
        Task<List<Product>> SearchProductsAsync(string searchTerm);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<List<Product>> GetLowStockProductsAsync();
        Task<bool> CheckStockAvailability(int productId, int quantity);
        Task UpdateStockAsync(int productId, int quantity);
        Task<decimal> GetTotalStockValueAsync();
    }
}
