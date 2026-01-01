using System;
using System.Collections.Generic;

namespace EyeHospitalPOS.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public DateTime? Birthday { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool ShowInSelectionList { get; set; } = true;

        // Navigation
        public ICollection<Sale>? Sales { get; set; }
    }
}
