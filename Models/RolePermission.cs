namespace EyeHospitalPOS.Models
{
    public class RolePermission
    {
        public int Id { get; set; }
        public string RoleId { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;

        // Navigation
        public Role? Role { get; set; }
    }
}
