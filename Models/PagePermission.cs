namespace EyeHospitalPOS.Models
{
    /// <summary>
    /// Represents a page/form in the application that can have permissions
    /// </summary>
    public class PagePermission
    {
        public int Id { get; set; }
        public string PageName { get; set; } = string.Empty;
        public string PagePath { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<RolePagePermission>? RolePagePermissions { get; set; }
    }
}
