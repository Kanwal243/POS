using System.Collections.Generic;

namespace EyeHospitalPOS.Models
{
    public class Role
    {
        public string Id { get; set; } ="";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsAdministrative { get; set; }
        public bool CanEditModel { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Parent-Child Role Hierarchy
        public string? ParentRoleId { get; set; }
        public Role? ParentRole { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<Role>? ChildRoles { get; set; }

        // Navigation
        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<User>? Users { get; set; }
        public ICollection<RolePermission>? RolePermissions { get; set; }
        public ICollection<RolePagePermission>? RolePagePermissions { get; set; }
        public ICollection<UserRoleAssignment>? UserRoleAssignments { get; set; }
    }
}
