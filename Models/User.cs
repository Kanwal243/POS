using System;
using System.Data;

namespace EyeHospitalPOS.Models
{
    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool ChangePasswordOnLogin { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        // Navigation
        [System.Text.Json.Serialization.JsonIgnore]
        public Role? Role { get; set; }
    }
}
