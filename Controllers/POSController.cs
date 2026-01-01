using EyeHospitalPOS.Models;
using EyeHospitalPOS.Interfaces;
using EyeHospitalPOS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Controllers
{
    public class POSController
    {
        private readonly IProductService _productService;
        private readonly ISalesService _salesService;

        public POSController(IProductService productService, ISalesService salesService)
        {
            _productService = productService;
            _salesService = salesService;
        }

        public async Task<List<Product>> SearchProductsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
            {
                return new List<Product>();
            }

            try
            {
                return await _productService.SearchProductsAsync(searchTerm);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Search error: {ex.Message}");
                return new List<Product>();
            }
        }

        public async Task<Product?> GetProductByBarcodeAsync(string barcode)
        {
            return await _productService.GetProductByBarcodeAsync(barcode);
        }

        public CartCalculation CalculateCart(List<CartItemDto> cartItems, decimal discountPercentage)
        {
            var subtotal = cartItems.Sum(c => c.Quantity * c.UnitPrice);
            var discountAmount = subtotal * (discountPercentage / 100);
            var totalAmount = subtotal - discountAmount;

            return new CartCalculation
            {
                Subtotal = subtotal,
                DiscountAmount = discountAmount,
                TotalAmount = totalAmount
            };
        }

        public async Task<(bool Success, string InvoiceNumber, string ErrorMessage)> CompleteSaleAsync(
            int customerId,
            string userId,
            List<CartItemDto> cartItems,
            decimal discountPercentage,
            string paymentMode)
        {
            if (!cartItems.Any())
            {
                return (false, string.Empty, "Cart is empty!");
            }

            try
            {
                var calculation = CalculateCart(cartItems, discountPercentage);

                var sale = new Sale
                {
                    CustomerId = customerId,
                    UserId = userId,
                    SaleDate = DateTime.Now,
                    SubTotal = calculation.Subtotal,
                    DiscountPercentage = discountPercentage,
                    DiscountAmount = calculation.DiscountAmount,
                    TotalAmount = calculation.TotalAmount,
                    PaymentMode = ParsePaymentMode(paymentMode),
                    SaleItems = cartItems.Select(c => new SaleItem
                    {
                        ProductId = c.ProductId,
                        Quantity = c.Quantity,
                        UnitPrice = c.UnitPrice,
                        Subtotal = c.Quantity * c.UnitPrice
                    }).ToList()
                };

                var createdSale = await _salesService.CreateSaleAsync(sale);
                return (true, createdSale.InvoiceNumber ?? string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, string.Empty, $"Error completing sale: {ex.Message}");
            }
        }

        private PaymentMode ParsePaymentMode(string mode)
        {
            return mode.Replace(" ", "") switch
            {
                "Cash" => PaymentMode.Cash,
                "Card" => PaymentMode.Card,
                "UPI" => PaymentMode.UPI,
                "BankTransfer" => PaymentMode.BankTransfer,
                _ => PaymentMode.Cash
            };
        }
    }

    public class CartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class CartCalculation
    {
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
