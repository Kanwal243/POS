using EyeHospitalPOS.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace EyeHospitalPOS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // User & Role Management
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PagePermission> PagePermissions { get; set; }
        public DbSet<RolePagePermission> RolePagePermissions { get; set; }
        public DbSet<UserRoleAssignment> UserRoleAssignments { get; set; }
        public DbSet<Organization> Organizations { get; set; }

        // Customer & Supplier
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        // Product Management
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; } // Matches your model name
        public DbSet<ProductType> ProductTypes { get; set; }

        // Sales & Invoice
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        // Purchase Orders
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }

        // Inventory
        public DbSet<InventoryReceiving> InventoryReceivings { get; set; }
        public DbSet<InventoryReceivingItem> InventoryReceivingItems { get; set; }
        public DbSet<ItemLocation> ItemLocations { get; set; }
        public DbSet<ProductLocation> ProductLocations { get; set; }

        // Dashboard Views
        public DbSet<DashboardSummaryView> DashboardSummary { get; set; }
        public DbSet<TopCustomerView> TopCustomers { get; set; }
        public DbSet<TopProductView> TopProducts { get; set; }
        public DbSet<CategorySalesView> CategorySales { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<DashboardSummaryView>().ToView("View_DashboardSummary").HasNoKey();
            builder.Entity<TopCustomerView>().ToView("View_TopCustomers").HasNoKey();
            builder.Entity<TopProductView>().ToView("View_TopProducts").HasNoKey();
            builder.Entity<CategorySalesView>().ToView("View_CategorySales").HasNoKey();

            // View Precisions
            builder.Entity<DashboardSummaryView>(entity => {
                entity.Property(v => v.PurchaseAmount).HasPrecision(18, 2);
                entity.Property(v => v.SalesAmount).HasPrecision(18, 2);
                entity.Property(v => v.StockAmount).HasPrecision(18, 2);
            });
            builder.Entity<TopCustomerView>(entity => entity.Property(v => v.TotalAmount).HasPrecision(18, 2));
            builder.Entity<TopProductView>(entity => entity.Property(v => v.TotalQuantity).HasPrecision(18, 2));
            builder.Entity<CategorySalesView>(entity => entity.Property(v => v.TotalQuantity).HasPrecision(18, 2));

            builder.Entity<Role>(entity => {
                entity.ToTable("Roles");
                entity.Property(e => e.Id).HasColumnType("nvarchar(450)");
            });

            builder.Entity<User>(entity => {
                entity.ToTable("Users");
                entity.Property(e => e.Id).HasColumnType("nvarchar(450)");
            });

            builder.Entity<RolePermission>()
                .Property(rp => rp.RoleId)
                .HasColumnType("nvarchar(450)");

            // Configure Role hierarchy (parent-child relationships)
            builder.Entity<Role>()
                .HasOne(r => r.ParentRole)
                .WithMany(r => r.ChildRoles)
                .HasForeignKey(r => r.ParentRoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Role>()
                .Property(r => r.ParentRoleId)
                .HasColumnType("nvarchar(450)");

            // Configure RolePagePermission
            builder.Entity<RolePagePermission>()
                .Property(rpp => rpp.RoleId)
                .HasColumnType("nvarchar(450)");

            builder.Entity<RolePagePermission>()
                .HasOne(rpp => rpp.Role)
                .WithMany(r => r.RolePagePermissions)
                .HasForeignKey(rpp => rpp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RolePagePermission>()
                .HasOne(rpp => rpp.PagePermission)
                .WithMany(pp => pp.RolePagePermissions)
                .HasForeignKey(rpp => rpp.PagePermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure UserRoleAssignment
            builder.Entity<UserRoleAssignment>()
                .Property(ura => ura.UserId)
                .HasColumnType("nvarchar(450)");

            builder.Entity<UserRoleAssignment>()
                .Property(ura => ura.RoleId)
                .HasColumnType("nvarchar(450)");

            builder.Entity<UserRoleAssignment>()
                .Property(ura => ura.AssignedBy)
                .HasColumnType("nvarchar(450)");

            builder.Entity<Sale>()
                .Property(s => s.UserId)
                .HasColumnType("nvarchar(450)");


            // 2. FIX: MULTIPLE CASCADE PATHS (Error: 0x80131904)
            // SQL Server prohibits multiple cascade paths to the same table.
            // Change Product relationships to Restrict to break cycles.
            builder.Entity<InventoryReceivingItem>()
                .HasOne(iri => iri.Product)
                .WithMany()
                .HasForeignKey(iri => iri.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PurchaseItem>()
                .HasOne(pi => pi.Product)
                .WithMany()
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SaleItem>()
                .HasOne(si => si.Product)
                .WithMany()
                .HasForeignKey(si => si.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. FIX: DECIMAL PRECISION WARNINGS
            // Sale Entity
            builder.Entity<Sale>(entity => {
                entity.Property(s => s.TotalAmount).HasPrecision(18, 2);
                entity.Property(s => s.DiscountAmount).HasPrecision(18, 2);
                entity.Property(s => s.DiscountPercentage).HasPrecision(18, 2);
                entity.Property(s => s.SubTotal).HasPrecision(18, 2);
                entity.Property(s => s.AmountPaid).HasPrecision(18, 2);
            });

            // SaleItem Entity
            builder.Entity<SaleItem>(entity => {
                entity.Property(si => si.Subtotal).HasPrecision(18, 2);
                entity.Property(si => si.UnitPrice).HasPrecision(18, 2);
                entity.Property(si => si.Discount).HasPrecision(18, 2);
            });

            // Inventory & Product Entities
            builder.Entity<InventoryReceiving>(entity => {
                entity.Property(ir => ir.TotalAmount).HasPrecision(18, 2);
            });

            builder.Entity<InventoryReceivingItem>(entity => {
                entity.Property(iri => iri.CostPrice).HasPrecision(18, 2);
                entity.Property(iri => iri.Subtotal).HasPrecision(18, 2);
            });

            builder.Entity<Product>(entity => {
                entity.Property(p => p.CostPrice).HasPrecision(18, 2);
                entity.Property(p => p.SalePrice).HasPrecision(18, 2);
            });

            builder.Entity<PurchaseItem>(entity => {
                entity.Property(pi => pi.UnitPrice).HasPrecision(18, 2);
                entity.Property(pi => pi.Amount).HasPrecision(18, 2);
            });

            builder.Entity<PurchaseOrder>(entity => {
                entity.Property(po => po.TotalAmount).HasPrecision(18, 2);
            });

            // 4. RELATIONSHIPS & CONSTRAINTS
            builder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Invoice>()
                .HasOne(i => i.Sale)
                .WithOne(s => s.Invoice)
                .HasForeignKey<Invoice>(i => i.SaleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed initial system data
            SeedData(builder);
        }

        private void SeedData(ModelBuilder builder)
        {
            // Seed Roles with specific responsibilities
            builder.Entity<Role>().HasData(
                new Role 
                { 
                    Id = "1", 
                    Name = "Administrator", 
                    Description = "Full system access with all permissions", 
                    IsAdministrative = true, 
                    CanEditModel = true,
                    IsActive = true,
                    ParentRoleId = null
                },
                new Role 
                { 
                    Id = "2", 
                    Name = "Inventory Admin", 
                    Description = "Full inventory management access", 
                    IsAdministrative = false, 
                    CanEditModel = true,
                    IsActive = true,
                    ParentRoleId = null
                },
                new Role 
                { 
                    Id = "3", 
                    Name = "Inventory Clerk", 
                    Description = "Inventory receiving and basic operations", 
                    IsAdministrative = false, 
                    CanEditModel = true,
                    IsActive = true,
                    ParentRoleId = null
                },
                new Role 
                { 
                    Id = "4", 
                    Name = "Inventory Reports Viewer", 
                    Description = "View inventory reports only", 
                    IsAdministrative = false, 
                    CanEditModel = false,
                    IsActive = true,
                    ParentRoleId = null
                },
                new Role 
                { 
                    Id = "5", 
                    Name = "Sales Clerk", 
                    Description = "Process sales and manage customers", 
                    IsAdministrative = false, 
                    CanEditModel = false,
                    IsActive = true,
                    ParentRoleId = null
                },
                new Role 
                { 
                    Id = "6", 
                    Name = "General User", 
                    Description = "Basic system access", 
                    IsAdministrative = false, 
                    CanEditModel = false,
                    IsActive = true,
                    ParentRoleId = null
                }
            );

            builder.Entity<User>().HasData(
                new User
                {
                    Id = "1",
                    UserName = "admin",
                    Email = "admin@eyehospital.com",
                    PasswordHash = "$2a$11$9uKV4bx3Pq0LxLJ6NZYMZeF8VqF7Z.AH8F7K5QH9W4Z8Q3N6Z8Q1W",
                    RoleId = "1",
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 12, 22)
                }
            );

            builder.Entity<Customer>().HasData(
                new Customer { Id = 1, FirstName = "Cash", LastName = "Walk-in", DisplayName = "Cash / Walk-in", Phone = "0000000000", IsActive = true, ShowInSelectionList = true }
            );

            // Seed Page Permissions
            builder.Entity<PagePermission>().HasData(
                // Dashboard
                new PagePermission { Id = 1, PageName = "Dashboard", PagePath = "/", Module = "Dashboard", Description = "Main dashboard view", IsActive = true },
                
                // Inventory Module
                new PagePermission { Id = 2, PageName = "Inventory Receiving", PagePath = "/inventory-receiving", Module = "Inventory", Description = "Receive inventory items", IsActive = true },
                new PagePermission { Id = 3, PageName = "Purchase Orders", PagePath = "/purchase-orders", Module = "Inventory", Description = "Manage purchase orders", IsActive = true },
                new PagePermission { Id = 4, PageName = "Products", PagePath = "/products", Module = "Inventory Setup", Description = "Manage products", IsActive = true },
                new PagePermission { Id = 5, PageName = "Product Categories", PagePath = "/product-categories", Module = "Inventory Setup", Description = "Manage product categories", IsActive = true },
                new PagePermission { Id = 6, PageName = "Product Types", PagePath = "/product-types", Module = "Inventory Setup", Description = "Manage product types", IsActive = true },
                new PagePermission { Id = 7, PageName = "Item Locations", PagePath = "/item-locations", Module = "Inventory Setup", Description = "Manage item locations", IsActive = true },
                new PagePermission { Id = 8, PageName = "Suppliers", PagePath = "/suppliers", Module = "Inventory Setup", Description = "Manage suppliers", IsActive = true },
                
                // Sales Module
                new PagePermission { Id = 9, PageName = "Point of Sale", PagePath = "/pos", Module = "Sales", Description = "Process sales transactions", IsActive = true },
                new PagePermission { Id = 10, PageName = "Invoices", PagePath = "/invoices", Module = "Sales", Description = "View and manage invoices", IsActive = true },
                new PagePermission { Id = 11, PageName = "Customers", PagePath = "/customers", Module = "Inventory Setup", Description = "Manage customers", IsActive = true },
                
                // Reports Module
                new PagePermission { Id = 12, PageName = "Reports", PagePath = "/reports", Module = "Reports", Description = "View system reports", IsActive = true },
                
                // System Setup
                new PagePermission { Id = 13, PageName = "Users", PagePath = "/users", Module = "System Setup", Description = "Manage system users", IsActive = true },
                new PagePermission { Id = 14, PageName = "Roles", PagePath = "/roles", Module = "Navigation", Description = "Manage user roles and permissions", IsActive = true },
                new PagePermission { Id = 15, PageName = "Organization Info", PagePath = "/info", Module = "System Setup", Description = "Manage organization information", IsActive = true },
                
                // User Profile
                new PagePermission { Id = 16, PageName = "Profile", PagePath = "/profile", Module = "Default", Description = "User profile management", IsActive = true },
                new PagePermission { Id = 17, PageName = "Change Password", PagePath = "/change-password", Module = "Default", Description = "Change user password", IsActive = true }
            );

            // Seed Organization Info
            builder.Entity<Organization>().HasData(
                new Organization
                {
                    Id = 1,
                    Name = "Eye Hospital POS",
                    Address = "123 Medical Street, Health City",
                    Phone = "+1 (555) 123-4567",
                    Email = "info@eyehospitalpos.com",
                    TaxId = "12-3456789",
                    LicenseNumber = "MED-2024-001",
                    Currency = "USD ($)",
                    TimeZone = "UTC-5 (Eastern Time)",
                    DateFormat = "MM/DD/YYYY"
                }
            );

            // Dummy Data for Barcode Testing
            builder.Entity<ProductCategory>().HasData(
                new ProductCategory { Id = 101, Name = "Frames", Code = "FRM", IsActive = true },
                new ProductCategory { Id = 102, Name = "Lenses", Code = "LNS", IsActive = true },
                new ProductCategory { Id = 103, Name = "Medicines", Code = "MED", IsActive = true }
            );

            builder.Entity<ProductType>().HasData(
                new ProductType { Id = 101, Name = "Optical" },
                new ProductType { Id = 102, Name = "Pharmaceutical" }
            );

            builder.Entity<Supplier>().HasData(
                new Supplier { Id = 101, Name = "Global Optical", IsActive = true, Phone = "123-456-7890", Email = "contact@globaloptical.com" },
                new Supplier { Id = 102, Name = "Pharma Care Ltd", IsActive = true, Phone = "098-765-4321", Email = "sales@pharmacare.com" }
            );

            builder.Entity<Product>().HasData(
                new Product { 
                    Id = 101, 
                    Name = "Ray-Ban Classic Frame", 
                    Barcode = "1234567890123", 
                    CategoryId = 101, 
                    TypeId = 101, 
                    SupplierId = 101, 
                    CostPrice = 120.00m, 
                    SalePrice = 180.00m, 
                    StockQuantity = 25, 
                    IsActive = true 
                },
                new Product { 
                    Id = 102, 
                    Name = "Anti-Reflective Lens (Pair)", 
                    Barcode = "9876543210987", 
                    CategoryId = 102, 
                    TypeId = 101, 
                    SupplierId = 101, 
                    CostPrice = 45.00m, 
                    SalePrice = 95.00m, 
                    StockQuantity = 50, 
                    IsActive = true 
                },
                new Product { 
                    Id = 103, 
                    Name = "Opti-Free Refreshing Eye Drops", 
                    Barcode = "4561237890456", 
                    CategoryId = 103, 
                    TypeId = 102, 
                    SupplierId = 102, 
                    CostPrice = 12.50m, 
                    SalePrice = 19.99m, 
                    StockQuantity = 100, 
                    IsActive = true 
                }
            );
        }
    }
}