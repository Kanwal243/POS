using EyeHospitalPOS.Models;
using EyeHospitalPOS.Interfaces;
using EyeHospitalPOS.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Controllers
{
    public class ProductController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            try
            {
                return await _productService.GetAllProductsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading products: {ex.Message}");
                return new List<Product>();
            }
        }

        public async Task<List<Product>> GetActiveProductsAsync()
        {
            try
            {
                return await _productService.GetActiveProductsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading active products: {ex.Message}");
                return new List<Product>();
            }
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productService.GetProductByIdAsync(id);
        }

        public async Task<(bool Success, Product? Product, string ErrorMessage)> CreateProductAsync(Product product)
        {
            try
            {
                var created = await _productService.CreateProductAsync(product);
                return (true, created, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, null, $"Error creating product: {ex.Message}");
            }
        }

        public async Task<(bool Success, Product? Product, string ErrorMessage)> UpdateProductAsync(Product product)
        {
            try
            {
                var updated = await _productService.UpdateProductAsync(product);
                return (true, updated, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, null, $"Error updating product: {ex.Message}");
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting product: {ex.Message}");
                return false;
            }
        }
    }
}
