using System.Collections.Generic;

namespace EyeHospitalPOS.Models
{
    public class ItemLocation
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // Requested Field
        public string Description { get; set; } = string.Empty;
        public int? ParentId { get; set; } // Requested Field
        public bool IsActive { get; set; } = true;

        // Navigation
        public ItemLocation? ParentLocation { get; set; }
        public ICollection<ItemLocation>? SubLocations { get; set; }
        public ICollection<ProductLocation>? ProductLocations { get; set; }
    }

    public class ProductLocation
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int LocationId { get; set; }
        public int StockQuantity { get; set; }

        // Navigation
        public Product? Product { get; set; }
        public ItemLocation? Location { get; set; }
    }
}
