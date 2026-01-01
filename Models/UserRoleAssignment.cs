using System;

namespace EyeHospitalPOS.Models
{
    /// <summary>
    /// Represents the assignment of a role to a user with additional metadata
    /// </summary>
    public class UserRoleAssignment
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string AssignedBy { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Navigation
        public User? User { get; set; }
        public Role? Role { get; set; }
    }
}
