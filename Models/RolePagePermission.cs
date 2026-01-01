namespace EyeHospitalPOS.Models
{
    /// <summary>
    /// Represents the permissions a role has for a specific page
    /// </summary>
    public class RolePagePermission
    {
        public int Id { get; set; }
        public string RoleId { get; set; } = string.Empty;
        public int PagePermissionId { get; set; }
        
        // CRUD + Navigate permissions
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool CanCreate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanNavigate { get; set; }

        // Navigation
        public Role? Role { get; set; }
        public PagePermission? PagePermission { get; set; }
    }
}
