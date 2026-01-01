using System;
using System.Collections.Generic;

namespace EyeHospitalPOS.Models
{
    public enum PurchaseOrderStatus
    {
        Draft = 0,
        Active = 1,
        Completed = 2,
        Cancelled = 3
    }

    public class PurchaseOrder
    {
        public int Id { get; set; }
        public string PONumber { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now; // Used as Created On
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public DateTime? ExpectedDate { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public decimal TotalAmount { get; set; }
        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
        public bool IsCancelled { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        // Navigation
        public Supplier? Supplier { get; set; }
        public User? ApprovedByUser { get; set; }
        public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    }

    public class PurchaseItem
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; } // Auto-calculated (Qty * Price)

        // Navigation
        public PurchaseOrder? PurchaseOrder { get; set; }
        public Product? Product { get; set; }
    }
}