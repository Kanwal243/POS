using System.Collections.Generic;

namespace EyeHospitalPOS.Models
{
    public class ProductType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation
        public ICollection<Product>? Products { get; set; }
    }
}
