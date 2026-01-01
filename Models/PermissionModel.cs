namespace EyeHospitalPOS.Models
{
    public class PermissionModel
    {
        // POS Permissions
        public bool CanAccessPOS { get; set; }
        public bool CanProcessSales { get; set; }
        public bool CanVoidSales { get; set; }
        public bool CanRefundSales { get; set; }
        public bool CanEditPrices { get; set; }
        public bool CanOverrideDiscounts { get; set; }
        
        // Inventory Permissions
        public bool CanAccessInventory { get; set; }
        public bool CanEditInventory { get; set; }
        public bool CanReceiveInventory { get; set; }
        public bool CanAdjustInventory { get; set; }
        public bool CanViewInventoryReports { get; set; }
        public bool CanCreatePurchaseOrders { get; set; }
        
        // Sales Permissions
        public bool CanAccessSales { get; set; }
        public bool CanViewSalesReports { get; set; }
        public bool CanViewSalesHistory { get; set; }
        public bool CanManageCustomers { get; set; }
        
        // System Permissions
        public bool CanManageUsers { get; set; }
        public bool CanManageRoles { get; set; }
        public bool CanManageProducts { get; set; }
        public bool CanManageSuppliers { get; set; }
        public bool CanViewSystemReports { get; set; }
    }
}
