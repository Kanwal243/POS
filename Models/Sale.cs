using DevExpress.Blazor.Pager.Internal;
using System;
using System.Collections.Generic;

namespace EyeHospitalPOS.Models
{
    public enum PaymentMode
    {
        Cash = 0,
        Card = 1,
        UPI = 2,
        BankTransfer = 3
    }

    public class Sale
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? UserId { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.Now;
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public PaymentMode PaymentMode { get; set; } = PaymentMode.Cash;
        public string? InvoiceNumber { get; set; }
        public string? PurchasedBy { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }

        // Navigation
        public Customer? Customer { get; set; }
        public User? User { get; set; }
        public ICollection<SaleItem>? SaleItems { get; set; }
        public Invoice? Invoice { get; set; }
    }

    public class SaleItem
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Subtotal { get; set; }
        public string? Description { get; set; }

        // Navigation
        public Sale? Sale { get; set; }
        public Product? Product { get; set; }
    }
}
