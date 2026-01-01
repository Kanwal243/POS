using System;
using System.Collections.Generic;

namespace EyeHospitalPOS.Models
{
    public enum InventoryStatus
    {
        Draft = 0,
        Active = 1,
        Cancelled = 2
    }

    public class InventoryReceiving
    {
        public int Id { get; set; }
        public string IRNumber { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public int? PurchaseOrderId { get; set; }
        public DateTime ReceivingDate { get; set; } = DateTime.Now;
        public string? Description { get; set; }
        public string? SupplierInvoiceNo { get; set; }
        public DateTime? SupplierInvoiceDate { get; set; }
        public string? ReceivedBy { get; set; }
        public decimal TotalAmount { get; set; }
        public InventoryStatus Status { get; set; } = InventoryStatus.Draft;
        public string? PostedBy { get; set; }
        public DateTime? PostedDate { get; set; }

        // Navigation
        public Supplier? Supplier { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }
        public User? PostedByUser { get; set; }
        public ICollection<InventoryReceivingItem> InventoryReceivingItems { get; set; } = new List<InventoryReceivingItem>();
    }

    public class InventoryReceivingItem
    {
        public int Id { get; set; }
        public int InventoryReceivingId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal Subtotal { get; set; }

        // Navigation
        public InventoryReceiving? InventoryReceiving { get; set; }
        public Product? Product { get; set; }
    }
}
