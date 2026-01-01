# Eye Hospital POS - Complete Project Documentation

## Table of Contents
1. [Project Overview](#project-overview)
2. [Core Features](#core-features)
3. [Page-by-Page Functionality](#page-by-page-functionality)
4. [Business Workflows](#business-workflows)
5. [Technical Architecture](#technical-architecture)
6. [Security & Authentication](#security--authentication)
7. [Reporting & Analytics](#reporting--analytics)

---

## Project Overview

The Eye Hospital POS is a comprehensive Point of Sale system built with .NET 8.0 and Blazor Server, designed specifically for eye hospitals and optical shops. It provides a complete solution for inventory management, sales processing, customer management, and reporting with role-based access control.

### Technology Stack
- **Backend**: .NET 8.0, Entity Framework Core
- **Frontend**: Blazor Server with DevExpress UI components
- **Database**: SQL Server with Code-First Migrations
- **Authentication**: JWT with Access/Refresh tokens
- **UI Framework**: DevExpress Blazor components
- **Hardware Integration**: Barcode scanner support

---

## Core Features

### 1. Core POS & Sales Management
*   **Touch-Friendly POS Interface**: Optimized screen for rapid transaction processing.
*   **Barcode Scanning**: Integrated search and auto-entry using barcode scanners.
*   **Cart Management**: Add/remove items, dynamic quantity adjustment, and stock-level validation.
*   **Discounting System**: Support for percentage-based discounts on the total payable amount.
*   **Multiple Payment Methods**: Support for Cash, Card, UPI, and Bank Transfer.
*   **Invoice Generation**: Automated generation of unique invoice numbers upon sale completion.
*   **Invoice History**: Dedicated module to search, view, and reprint previous transactions.
*   **Customer Management**: Profiles for walk-in customers and registered patients.

### 2. Advanced Inventory & Procurement
*   **Product Catalog**: Robust management of frames, lenses, medicines, and services.
*   **Multi-Attribute Products**: Support for Category, Product Type, and Supplier associations.
*   **Stock Tracking**: Real-time monitoring of stock levels with visual alerts for low inventory.
*   **Purchase Orders (PO)**: Complete procurement lifecycle management (Draft -> Active -> Completed/Cancelled).
*   **Inventory Receiving (IR)**: Formalized stock-in process to update inventory levels, linked to POs or standalone.
*   **Supplier Management**: Comprehensive registry of vendors and their contact information.
*   **Reorder Alerts**: Configurable reorder levels for each product to prevent stockouts.

### 3. System Administration & Security
*   **Regional Settings**: Dynamic currency selection (USD, PKR, EUR, etc.) that reflects throughout the app.
*   **Organization Profile**: Centralized hospital identity management (Logo, Name, Address, Contact).
*   **Role-Based Access Control (RBAC)**:
    *   **Page-Level Permissions**: Control who can see specific modules.
    *   **Action-Level Permissions**: Manage Add/Edit/Delete capabilities per role.
    *   **Role Hierarchy**: Support for parent-child role structures.
*   **JWT Authentication**: Secure, industrial-standard login with access and refresh token logic.
*   **Security Compliance**: Required password changes for new users on their first login.
*   **User Management**: Full CRUD operations for system users with status (Active/Inactive) control.

### 4. Reporting & Analytics
*   **Interactive Dashboard**: Real-time visual metrics including:
    *   Total Sales Revenue
    *   Total Order Volume
    *   Customer Growth Trends
*   **Top Performance Insights**: Identification of top-selling products and highest-value customers.
*   **Category Sales**: Breakdown of revenue across different product departments.
*   **Audit Logs**: (Infrastructure ready) Tracking of significant system changes.

### 5. Technical Highlights
*   **Modern Technology Stack**: Built with .NET 8.0 and Blazor Server.
*   **DevExpress UI**: Premium, high-performance UI components for data grids and forms.
*   **Scalable Architecture**: Interface-driven service layer (SOLID principles) for easy maintenance.
*   **Responsive Design**: Modern, premium CSS theme with sidebar navigation and mobile-friendly layouts.
*   **Hardware Ready**: Built-in support for barcode scanners and printer-ready invoice templates.

---

## Page-by-Page Functionality

### 🚀 Dashboard (`/dashboard`)
The central command hub for a quick business overview.
*   **Total Customers Card**: Displays total registered patients/customers with a growth trend percentage.
*   **Total Orders Card**: Summarizes the total sales orders processed.
*   **Dynamic Currency Support**: All financial figures automatically adapt to the organization's currency symbol.
*   **Real-time Analytics**: (Infrastructure for) Top selling products and customer performance charts.

### 📦 Products Catalog (`/product`)
The master registry of all inventory items.
*   **Premium Data Grid**: Filterable, searchable, and paginated list of all products.
*   **Inventory Status**: Visual badges indicating "Active" or "Inactive" status.
*   **Dual Scanning Modes**:
    *   **External Scanner**: Modal interface to quickly look up product details via barcode.
    *   **Form Auto-fill**: Inline scanner support within the Add/Edit form to populate product data instantly.
*   **Pricing Management**: Input fields for Cost Price and Sale Price with dynamic currency formatting.
*   **Stock Monitoring**: Integrated stock quantity and reorder level indicators.

### 🚛 Inventory Receiving (`/inventory-receiving`)
Tracks the arrival of goods from suppliers.
*   **Batch Processing**: Record bulk arrivals of products into the system.
*   **Detailed IR Form**:
    *   Supplier selection.
    *   Transaction date and reference number management.
    *   Line-item entry with unit cost and subtotal calculations.
*   **Status Management**: Distinguishes between "Draft" arrivals and "Active" confirmed stock-ins.
*   **Automatic Stock Update**: Confirming an IR record instantly increments the product warehouse quantities.

### 📑 Purchase Orders (`/purchase-order`)
The procurement management module.
*   **Procurement Workflow**: Management of orders from Draft to Active, and eventually Completed or Cancelled.
*   **PO Lifecycle**:
    *   Drafting with product selection and cost negotiation.
    *   Activation to finalize order requirements.
    *   Generating an Inventory Receiving (IR) record directly from an active PO to minimize manual entry.
*   **Summary Footer**: Real-time total items count and total payable amount calculations.

### 🧾 Sales & Invoices (`/invoices`)
The historical registry of customer billing.
*   **Audit Trail**: Searchable list of all generated invoices with unique `InvoiceNumber`.
*   **Customer Linking**: Direct visibility into which customer/patient was billed for each transaction.
*   **Status Tracking**: Indicators for "Printed" vs "Draft" invoices.
*   **Financial Visibility**: Clear display of total sale amounts with dynamic currency support.

### 👥 Customer & Supplier Management

#### Customers (`/customers`)
*   **Patient Registry**: Management of name, phone, email, and display names.
*   **Status Control**: Toggle customers between Active and Inactive.
*   **Quick Search**: Find customers by phone or name for rapid POS checkout.

#### Suppliers (`/suppliers`)
*   **Vendor Management**: Track contact details, addresses, and status of inventory providers.
*   **Supplier Linking**: Associated with Products and Purchase Orders for streamlined procurement.

### ⚙️ System Setup & Security

#### Organization Info (`/info`)
*   **White-labeling**: Configure hospital name, logo, phone, and address.
*   **Global Localization**: The **sole source of truth** for:
    *   Base Currency (e.g., USD, PKR).
    *   Date Display Format.
    *   Local Time Zone.

#### User Management (`/users`)
*   **Account Controls**: Create, update, and manage system users.
*   **Security Protocol**: Toggle "Change Password on Login" for new staff members.

#### Role Management (`/roles`)
*   **Granular RBAC**: Assign page-level visibility and action-level permissions (CanEdit, CanAdd, etc.).
*   **Hierarchy**: Supports parent-child role relations for complex organizational structures.

### 🔐 Personal Settings

#### Profile (`/profile`)
*   **Self-Service**: Users can update their personal information and contact details.
*   **Session Info**: Displays assigned roles and system permissions.

#### Change Password (`/change-password`)
*   **Security Check**: Self-service password updates with validation checks.

---

## Business Workflows

### 1. System Initialization & Setup
The foundation of the application configuration.

*   **Organization Configuration**: Set hospital name, contact details, and localization settings (Currency, Date Format, Time Zone) in `System Setup > Organization Info`.
*   **Access Control**: 
    *   **Roles & Permissions**: Define roles (e.g., Administrator, Sales Clerk, Inventory Admin) and assign granular permissions for pages and modules.
    *   **User Management**: Create system users and assign them specific roles.
*   **Security**: Enforce password policies, including mandatory password changes on first login for new users.

### 2. Inventory Management Workflow
The cycle of managing ophthalmic products and stock levels.

#### A. Product Catalog Setup
1.  Define **Product Categories** (e.g., Frames, Lenses, Medicine).
2.  Define **Product Types** (e.g., Single Vision, Bifocal).
3.  Register **Suppliers** who provide the inventory.

#### B. Procurement (Purchase Orders)
1.  **Draft**: Create a new Purchase Order (PO) by selecting a supplier and adding products.
2.  **Activation**: Review and activate the PO to authorize the order.
3.  **Completion/IR**: Once items arrive, generate an **Inventory Receiving (IR)** record from the PO.

#### C. Inventory Receiving (Stock In)
*   **Direct Receiving**: Stock can also be received without a PO.
*   **Stock Update**: Activating an Inventory Receiving record automatically increments the `StockQuantity` of the associated products in the database.

### 3. Sales & Billing Workflow
The daily operations for processing patient transactions.

1.  **Create Invoice**: Navigate to `Sales > Invoices` and click **"New Invoice"**.
2.  **Order Entry**:
    *   Select or Search for a **Customer**.
    *   Add products to the **Issued Items** list.
    *   Adjust quantities and unit prices as needed.
3.  **Financials**:
    *   Apply any applicable **Discounts** (item-level or total).
    *   Record the **Amount Paid** by the customer.
4.  **Transaction Processing**:
    *   Click **"Save"** to persist the sale and generate a unique `InvoiceNumber`.
    *   The system automatically deducts sold items from `StockQuantity`.
5.  **Billing**: Preview or Print the generated invoice directly from the form or history list.

### 4. Daily Operations Workflow
*   **Opening**: Staff logs in using their credentials and assigned role permissions.
*   **Sales Processing**: Process customer orders through the POS interface, with real-time stock validation.
*   **Inventory Monitoring**: Regular checks for low stock levels and creation of purchase orders.
*   **Reporting**: Generate daily sales reports and inventory status updates.
*   **Closing**: End-of-day reconciliation and data backup.

---

## Technical Architecture

### Service Layer Architecture
The application follows a clean, interface-driven service architecture:

*   **Authentication**: Managed via `IAuthService` and JWT tokens. Handles login, token refresh, and user state.
*   **Hardware Integration**: `IBarcodeService` handles barcode generation and decoding using ZXing.
*   **Inventory Logic**: `IInventoryService` and `IPurchaseOrderService` manage complex stock transactions and state transitions.
*   **Identity Context**: `IHttpContextService` provides abstracted access to the current user's identity and claims throughout the service layer.
*   **UI Components**: Utilizes **DevExpress Blazor Components** for high-performance grids, editors, and popups.

### Data Models
*   **User**: Core user model with role associations and security flags
*   **Role**: Hierarchical role model with permissions and administrative flags
*   **Product**: Inventory item with category, type, supplier, pricing and stock information
*   **Sale**: Transaction model with customer, items, pricing and payment information
*   **Customer**: Patient/customer profile with contact information
*   **Supplier**: Vendor information for procurement
*   **PurchaseOrder**: Procurement workflow model
*   **InventoryReceiving**: Stock-in tracking model
*   **Invoice**: Billing document model

### Controllers
*   **API Controllers**: RESTful endpoints for data operations
    *   `AuthController` - Authentication endpoints
    *   `InventoryController` - Inventory management
    *   `ProductsController` - Product catalog
    *   `SalesController` - Sales transactions
    *   `UsersController` - User management
*   **MVC Controllers**: Traditional server-side rendering where needed
    *   `LoginController` - Login handling
    *   `DashboardController` - Dashboard data
    *   `ProductController` - Product operations
    *   `UserController` - User operations

### Middleware
*   `HttpContextLoggingMiddleware` - Request logging and correlation
*   Session management for Blazor Server state
*   Authentication and Authorization middleware

---

## Security & Authentication

### JWT Token Management
*   **Access Tokens**: Short-lived (60 minutes) for API requests
*   **Refresh Tokens**: Longer-lived (7 days) for automatic token refresh
*   **Token Rotation**: Secure refresh token rotation to prevent replay attacks
*   **Automatic Refresh**: Transparent token refresh when access token expires

### Role-Based Access Control (RBAC)
*   **Page-Level Permissions**: Control who can access specific pages/modules
*   **Action-Level Permissions**: Granular control over Add/Edit/Delete operations
*   **Role Hierarchy**: Parent-child role relationships for complex organizational structures
*   **Administrative Roles**: Special flags for system administration tasks

### Security Features
*   **Password Policies**: Enforced complexity and change requirements
*   **First Login Requirement**: Force password change for new users
*   **Session Management**: HTTP Session for Blazor Server authentication state
*   **Secure Cookies**: HttpOnly and Secure flags for session cookies
*   **Anti-Forgery Protection**: XSRF token validation

---

## Reporting & Analytics

### Dashboard Metrics
*   **Total Customers**: Count with growth trend
*   **Total Orders**: Sales volume tracking
*   **Revenue Analytics**: Sales revenue with currency support
*   **Top Products**: Best selling items visualization
*   **Top Customers**: High-value customer identification
*   **Category Sales**: Revenue breakdown by product category

### Report Types
*   **Sales Reports**: Daily, weekly, monthly sales summaries
*   **Inventory Reports**: Stock levels, low stock alerts, reorder reports
*   **Customer Reports**: Customer activity and purchasing patterns
*   **Financial Reports**: Revenue, profit margins, payment methods
*   **Audit Reports**: User activity logs and system changes

### Data Visualization
*   **Interactive Charts**: Real-time data visualization
*   **Filtering Options**: Date ranges, categories, and custom filters
*   **Export Capabilities**: PDF, Excel, and CSV exports
*   **Print-Ready Formats**: Professional invoice and report templates

---

*Last Updated: December 2025*