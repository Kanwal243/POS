using System.Collections.Generic;

namespace EyeHospitalPOS.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // From Screenshot
        public int? ParentId { get; set; } // From Screenshot (Parent Category)
        public bool IsActive { get; set; } = true; // From Screenshot (Status)
        public bool ShowInSelectionList { get; set; } = true; // From Screenshot

        // Navigation
        public ProductCategory? ParentCategory { get; set; }
        public ICollection<ProductCategory>? SubCategories { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
