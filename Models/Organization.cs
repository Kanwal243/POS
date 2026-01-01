using System.ComponentModel.DataAnnotations;

namespace EyeHospitalPOS.Models
{
    public class Organization
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? TaxId { get; set; }

        [StringLength(50)]
        public string? LicenseNumber { get; set; }

        [StringLength(20)]
        public string Currency { get; set; } = "PKR";

        [StringLength(100)]
        public string TimeZone { get; set; } = "UTC-5 (Eastern Time)";

        [StringLength(20)]
        public string DateFormat { get; set; } = "MM/DD/YYYY";
        
        public string? LogoPath { get; set; }
    }
}
