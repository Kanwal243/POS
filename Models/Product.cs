namespace EyeHospitalPOS.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int TypeId { get; set; }
        public int SupplierId { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; } = 10;
        public bool IsActive { get; set; } = true;

        // Navigation
        public ProductCategory? Category { get; set; }
        public ProductType? Type { get; set; }
        public Supplier? Supplier { get; set; }
    }
}
