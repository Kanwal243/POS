# Eye Hospital POS - Complete Project Documentation

## Table of Contents
1. [Project Overview](#project-overview)
2. [Core Features](#core-features)
3. [Page-by-Page Functionality](#page-by-page-functionality)
4. [Business Workflows](#business-workflows)
5. [Technical Architecture](#technical-architecture)
6. [Security & Authentication](#security--authentication)
7. [Reporting & Analytics](#reporting--analytics)
8. [JWT Authentication API](#jwt-authentication-api)
9. [HttpContextAccessor Implementation](#httpcontextaccessor-implementation)
10. [Implementation Summary](#implementation-summary)
11. [JWT Quick Reference](#jwt-quick-reference)

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

# JWT Authentication API

## Overview

This document provides comprehensive information about the JWT-based Authentication and Role-Based Authorization API for the EyeHospitalPOS system.

## Base URL
```
https://localhost:5001/api
```

## Authentication

All protected endpoints require a valid JWT token in the Authorization header:
```
Authorization: Bearer {your_jwt_token}
```

---

## API Endpoints

### 1. Authentication Endpoints

#### POST `/api/auth/register`
Register a new user account.

**Request Body:**
```json
{
  "userName": "john.doe",
  "email": "john.doe@example.com",
  "password": "SecurePassword123",
  "confirmPassword": "SecurePassword123",
  "roleId": "6"  // Optional, defaults to General User
}
```

**Response (Success - 200 OK):**
```json
{
  "success": true,
  "message": "User registered successfully",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123def456...",
  "tokenExpiration": "2025-12-28T01:00:00Z",
  "user": {
    "id": "user-guid-here",
    "userName": "john.doe",
    "email": "john.doe@example.com",
    "roleId": "6",
    "roleName": "General User",
    "isActive": true,
    "createdDate": "2025-12-27T20:00:00Z"
  }
}
```

#### POST `/api/auth/login`
Authenticate user and receive JWT tokens.

**Request Body:**
```json
{
  "userName": "admin",
  "password": "Admin@123"
}
```

**Response (Success - 200 OK):**
```json
{
  "success": true,
  "message": "Login successful",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "xyz789abc456...",
  "tokenExpiration": "2025-12-27T21:00:00Z",
  "user": {
    "id": "1",
    "userName": "admin",
    "email": "admin@eyehospital.com",
    "roleId": "1",
    "roleName": "Administrator",
    "isActive": true,
    "createdDate": "2025-12-22T00:00:00Z"
  }
}
```

**Response (Error - 401 Unauthorized):**
```json
{
  "success": false,
  "message": "Invalid username or password"
}
```

#### POST `/api/auth/refresh-token`
Refresh expired access token using refresh token.

**Request Body:**
```json
{
  "accessToken": "expired_access_token_here",
  "refreshToken": "valid_refresh_token_here"
}
```

**Response (Success - 200 OK):**
```json
{
  "success": true,
  "message": "Token refreshed successfully",
  "accessToken": "new_access_token_here",
  "refreshToken": "new_refresh_token_here",
  "tokenExpiration": "2025-12-27T22:00:00Z",
  "user": {
    "id": "1",
    "userName": "admin",
    "email": "admin@eyehospital.com",
    "roleId": "1",
    "roleName": "Administrator",
    "isActive": true,
    "createdDate": "2025-12-22T00:00:00Z"
  }
}
```

#### POST `/api/auth/revoke-token`
Revoke all refresh tokens for the current user (Logout).

**Headers:**
```
Authorization: Bearer {your_jwt_token}
```

**Response (Success - 200 OK):**
```json
{
  "message": "Tokens revoked successfully"
}
```

#### POST `/api/auth/change-password`
Change password for authenticated user.

**Headers:**
```
Authorization: Bearer {your_jwt_token}
```

**Request Body:**
```json
{
  "currentPassword": "OldPassword123",
  "newPassword": "NewPassword456",
  "confirmPassword": "NewPassword456"
}
```

**Response (Success - 200 OK):**
```json
{
  "message": "Password changed successfully"
}
```

#### GET `/api/auth/me`
Get current authenticated user information (Protected Route Example).

**Headers:**
```
Authorization: Bearer {your_jwt_token}
```

**Response (Success - 200 OK):**
```json
{
  "userId": "1",
  "userName": "admin",
  "email": "admin@eyehospital.com",
  "role": "Administrator",
  "claims": [
    { "type": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "value": "1" },
    { "type": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "value": "admin" },
    { "type": "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "value": "Administrator" }
  ]
}
```

---

### 2. Role Management Endpoints (Admin Only)

#### GET `/api/roles`
Get all roles in the system.

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Response (Success - 200 OK):**
```json
[
  {
    "id": "1",
    "name": "Administrator",
    "description": "Full system access with all permissions",
    "isAdministrative": true
  },
  {
    "id": "2",
    "name": "Inventory Admin",
    "description": "Full inventory management access",
    "isAdministrative": false
  }
]
```

#### GET `/api/roles/{id}`
Get role details by ID.

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Response (Success - 200 OK):**
```json
{
  "id": "1",
  "name": "Administrator",
  "description": "Full system access with all permissions",
  "isAdministrative": true
}
```

#### POST `/api/roles`
Create a new role.

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Request Body:**
```json
{
  "name": "Store Manager",
  "description": "Manages store operations",
  "isAdministrative": false,
  "canEditModel": true
}
```

**Response (Success - 201 Created):**
```json
{
  "id": "7",
  "name": "Store Manager",
  "description": "Manages store operations",
  "isAdministrative": false
}
```

#### PUT `/api/roles/{id}`
Update an existing role.

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Request Body:**
```json
{
  "name": "Updated Role Name",
  "description": "Updated description",
  "isAdministrative": false,
  "canEditModel": true
}
```

**Response (Success - 200 OK):**
```json
{
  "id": "7",
  "name": "Updated Role Name",
  "description": "Updated description",
  "isAdministrative": false
}
```

#### DELETE `/api/roles/{id}`
Delete a role.

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Response (Success - 200 OK):**
```json
{
  "message": "Role deleted successfully"
}
```

#### POST `/api/roles/assign`
Assign a role to a user.

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Request Body:**
```json
{
  "userId": "user-guid-here",
  "roleId": "2"
}
```

**Response (Success - 200 OK):**
```json
{
  "message": "Role assigned successfully",
  "userId": "user-guid-here",
  "userName": "john.doe",
  "roleId": "2",
  "roleName": "Inventory Admin"
}
```

---

### 3. User Management Endpoints

#### GET `/api/users`
Get all users (Admin only).

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Response (Success - 200 OK):**
```json
[
  {
    "id": "1",
    "userName": "admin",
    "email": "admin@eyehospital.com",
    "roleId": "1",
    "roleName": "Administrator",
    "isActive": true,
    "createdDate": "2025-12-22T00:00:00Z"
  }
]
```

#### GET `/api/users/{id}`
Get user by ID (Admin or self).

**Headers:**
```
Authorization: Bearer {your_jwt_token}
```

**Authorization:** User can view their own profile, Administrators can view any profile

**Response (Success - 200 OK):**
```json
{
  "id": "1",
  "userName": "admin",
  "email": "admin@eyehospital.com",
  "roleId": "1",
  "roleName": "Administrator",
  "isActive": true,
  "createdDate": "2025-12-22T00:00:00Z"
}
```

#### PUT `/api/users/{id}/activate`
Activate a deactivated user (Admin only).

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Response (Success - 200 OK):**
```json
{
  "message": "User activated successfully"
}
```

#### PUT `/api/users/{id}/deactivate`
Deactivate a user (Admin only).

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Response (Success - 200 OK):**
```json
{
  "message": "User deactivated successfully"
}
```

#### DELETE `/api/users/{id}`
Delete a user (Admin only).

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Response (Success - 200 OK):**
```json
{
  "message": "User deleted successfully"
}
```

---

## Available Roles

| ID | Role Name | Description | Capabilities |
|----|-----------|-------------|--------------|
| 1 | Administrator | Full system access with all permissions | Full Access |
| 2 | Inventory Admin | Full inventory management access | Inventory Management |
| 3 | Inventory Clerk | Inventory receiving and basic operations | Inventory Operations |
| 4 | Inventory Reports Viewer | View inventory reports only | Read-Only Inventory |
| 5 | Sales Clerk | Process sales and manage customers | Sales Operations |
| 6 | General User | Basic system access | Limited Access |

---

## JWT Token Details

### Access Token
- **Expiration:** 60 minutes
- **Includes Claims:**
  - User ID
  - Username
  - Email
  - Role Name
  - Role ID
  - IsAdministrative flag

### Refresh Token
- **Expiration:** 7 days
- **Usage:** Used to obtain new access tokens without re-authentication
- **Security:** Stored in database, can be revoked

---

## Error Responses

### 400 Bad Request
```json
{
  "success": false,
  "message": "Validation error message here"
}
```

### 401 Unauthorized
```json
{
  "success": false,
  "message": "Invalid credentials or token"
}
```

### 403 Forbidden
```json
{
  "message": "You do not have permission to access this resource"
}
```

### 404 Not Found
```json
{
  "message": "Resource not found"
}
```

---

## Example Usage

### Using cURL

**Login:**
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"userName":"admin","password":"Admin@123"}'
```

**Get Current User:**
```bash
curl -X GET https://localhost:5001/api/auth/me \
  -H "Authorization: Bearer your_jwt_token_here"
```

**Create Role (Admin Only):**
```bash
curl -X POST https://localhost:5001/api/roles \
  -H "Authorization: Bearer admin_jwt_token_here" \
  -H "Content-Type: application/json" \
  -d '{"name":"New Role","description":"Description here","isAdministrative":false,"canEditModel":true}'
```

### Using JavaScript/Fetch

```javascript
// Login
const login = async () => {
  const response = await fetch('https://localhost:5001/api/auth/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      userName: 'admin',
      password: 'Admin@123'
    })
  });
  
  const data = await response.json();
  
  if (data.success) {
    localStorage.setItem('accessToken', data.accessToken);
    localStorage.setItem('refreshToken', data.refreshToken);
  }
};

// Make authenticated request
const getMe = async () => {
  const token = localStorage.getItem('accessToken');
  
  const response = await fetch('https://localhost:5001/api/auth/me', {
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });
  
  const data = await response.json();
  console.log(data);
};
```

---

## Security Best Practices

1. **Always use HTTPS** in production
2. **Store tokens securely** (HttpOnly cookies or secure storage)
3. **Implement token rotation** using refresh tokens
4. **Validate all user input** on both client and server
5. **Use strong passwords** (minimum 6 characters, alphanumeric recommended)
6. **Revoke tokens** on logout
7. **Monitor failed login attempts**
8. **Implement rate limiting** to prevent brute force attacks

---

## Swagger/OpenAPI Documentation

This API includes Swagger documentation accessible at:
```
https://localhost:5001/swagger
```

You can test all endpoints directly from the Swagger UI in development mode.

---

## Support

For issues or questions, please contact the development team.

---

# HttpContextAccessor Implementation Guide

## Overview

This document describes the implementation of `IHttpContextAccessor` in the EyeHospitalPOS application with DevExpress Blazor. The implementation provides access to HTTP context information from anywhere in the application, including services, components, and middleware.

## Components

### 1. HttpContextService (`Services/HttpContextService.cs`)

A wrapper service that provides easy access to HTTP context information. This service encapsulates `IHttpContextAccessor` and provides convenient methods for common operations.

**Key Features:**
- Access to current user claims (ID, username, email, role)
- Request/response information (headers, cookies, query parameters)
- Client information (IP address, user agent, device detection)
- URL information (base URL, full URL, path)
- Cookie management (get, set, delete)

**Usage Example:**
```csharp
[Inject] private HttpContextService HttpContextService { get; set; }

// Get current user ID
var userId = HttpContextService.GetUserId();

// Get client IP address
var ipAddress = HttpContextService.GetClientIpAddress();

// Check if user is authenticated
if (HttpContextService.IsAuthenticated)
{
    // Perform authenticated operations
}
```

### 2. ApiClientService Integration

The `ApiClientService` base class now uses `HttpContextService` to:
- Automatically get JWT tokens from HTTP context when sessionStorage is unavailable
- Add correlation IDs to API requests for request tracking
- Access request headers and context information

### 3. HttpContextLoggingMiddleware (`Middleware/HttpContextLoggingMiddleware.cs`)

A custom middleware that demonstrates using HTTP context for:
- Request logging with correlation IDs
- Performance tracking (request duration)
- Error logging with context information

## Registration in Startup.cs

```csharp
// Register IHttpContextAccessor
services.AddHttpContextAccessor();

// Register HttpContextService wrapper
services.AddScoped<HttpContextService>();

// Configure HttpClient with HttpContextAccessor support
services.AddHttpClient("ApiClient", (sp, client) =>
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var request = httpContextAccessor.HttpContext?.Request;
    
    if (request != null)
    {
        var baseUrl = $"{request.Scheme}://{request.Host}";
        client.BaseAddress = new Uri(baseUrl);
    }
});
```

## Use Cases

### 1. Authentication & Authorization
```csharp
// Get current user information
var userId = HttpContextService.GetUserId();
var userRole = HttpContextService.GetUserRole();

// Check authentication status
if (HttpContextService.IsAuthenticated)
{
    // User is logged in
}
```

### 2. Request Tracking & Logging
```csharp
// Get correlation ID for request tracking
var correlationId = HttpContextService.GetCorrelationId();

// Log request information
_logger.LogInformation("Request from {IpAddress} by {UserId}", 
    HttpContextService.GetClientIpAddress(), 
    HttpContextService.GetUserId());
```

### 3. Cookie Management
```csharp
// Get cookie value
var theme = HttpContextService.GetCookie("theme");

// Set cookie
HttpContextService.SetCookie("theme", "dark", new CookieOptions
{
    HttpOnly = false,
    Secure = true,
    SameSite = SameSiteMode.Strict,
    Expires = DateTimeOffset.UtcNow.AddDays(30)
});

// Delete cookie
HttpContextService.DeleteCookie("theme");
```

### 4. Query Parameters & Headers
```csharp
// Get query parameter
var page = HttpContextService.GetQueryParameter("page");

// Get request header
var userAgent = HttpContextService.GetUserAgent();
var acceptLanguage = HttpContextService.GetRequestHeader("Accept-Language");
```

### 5. Device Detection
```csharp
// Check if request is from mobile device
if (HttpContextService.IsMobileDevice())
{
    // Optimize UI for mobile
}
```

## Best Practices

1. **Always check for null**: HTTP context may not be available in all scenarios (background services, signalR hubs, etc.)
   ```csharp
   if (HttpContextService.HttpContext != null)
   {
       // Safe to use HTTP context
   }
   ```

2. **Use HttpContextService instead of direct IHttpContextAccessor**: The wrapper provides better error handling and convenience methods.

3. **Be aware of scope**: HTTP context is only available during request processing. Don't try to access it from background services or outside request scope.

4. **Use correlation IDs**: Always include correlation IDs in logs and API requests for better traceability.

## Integration with DevExpress Blazor

The implementation works seamlessly with DevExpress Blazor components. HTTP context information can be accessed from:
- Blazor components (via dependency injection)
- DevExpress services
- Custom middleware
- API controllers

## Notes

- `IHttpContextAccessor` is registered as a singleton, but `HttpContextService` is registered as scoped to match the request lifecycle.
- HTTP context is not available during prerendering. Always check for null or use try-catch blocks.
- For Blazor Server, HTTP context is available in components. For Blazor WebAssembly, use JSRuntime for client-side operations.

---

# Implementation Summary

## ✅ Implementation Complete

A comprehensive JWT-based Authentication and Role-Based Authorization API has been successfully integrated into the EyeHospitalPOS application (.NET 8).

---

## 🎯 Features Implemented

### 1. **JWT Authentication**
- ✅ JWT Access Token generation (60-minute expiration)
- ✅ JWT Refresh Token mechanism (7-day expiration)
- ✅ Token validation and expiration handling
- ✅ Secure token storage in database

### 2. **User Registration & Login**
- ✅ Secure user registration with validation
- ✅ Password hashing (ready for BCrypt in production)
- ✅ Email validation
- ✅ User login with JWT token generation
- ✅ Failed login handling

### 3. **Role-Based Authorization**
- ✅ 6 Predefined Roles:
  - Administrator (ID: 1) - Full system access
  - Inventory Admin (ID: 2) - Full inventory management
  - Inventory Clerk (ID: 3) - Inventory operations
  - Inventory Reports Viewer (ID: 4) - Read-only inventory
  - Sales Clerk (ID: 5) - Sales operations
  - General User (ID: 6) - Basic access
- ✅ Role creation, update, and deletion (Admin only)
- ✅ Role assignment to users

### 4. **Protected Endpoints**
- ✅ Admin-only endpoints (`[Authorize(Roles = "Administrator")]`)
- ✅ User-specific access control
- ✅ Public endpoints for registration and login
- ✅ User profile management

### 5. **Security Features**
- ✅ Password change functionality
- ✅ Token revocation (logout)
- ✅ Refresh token rotation
- ✅ isAdministrative flag for special permissions
- ✅ Anti-self-harm protections (can't delete/deactivate self)

---

## 📁 Files Created/Modified

### New Files:
1. **Models/**
   - `RefreshToken.cs` - Refresh token model
   - `DTOs/AuthDtos.cs` - DTOs for all auth operations

2. **Controllers/Api/**
   - `AuthController.cs` - Authentication endpoints
   - `RolesController.cs` - Role management endpoints
   - `UsersController.cs` - User management endpoints

3. **Services/**
   - Updated `JwtService.cs` - Enhanced with refresh token support
   - Updated `AuthService.cs` - Complete auth logic with DTOs

4. **Documentation/**
   - `API_DOCUMENTATION.md` - Comprehensive API docs
   - `IMPLEMENTATION_SUMMARY.md` - This file

### Modified Files:
1. `Data/ApplicationDbContext.cs` - Added RefreshTokens DbSet
2. `Interfaces/IAuthService.cs` - Updated interface
3. `appsettings.json` - JWT configuration 
4. `Controllers/LoginController.cs` - Updated method names
5. `Pages/ChangePassword.razor` - Updated to use DTOs
6. Database Migration: `AddRefreshTokenAndUpdateRoles`

---

## 🔑 JWT Configuration

```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyForJWTTokenGeneration_AtLeast32Characters!",
    "Issuer": "EyeHospitalPOS",
    "Audience": "EyeHospitalPOS-API",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

---

## 🔐 Authentication Flow

### Registration/Login Flow:
```
┌─────────┐         ┌────────────┐        ┌──────────┐
│ Client  │────────►│   API      │───────►│ Database │
└─────────┘         └────────────┘        └──────────┘
     │                    │                      │
     │  POST /register    │   Create User        │
     ├───────────────────►│─────────────────────►│
     │                    │                      │
     │  POST /login       │   Validate User      │
     ├───────────────────►│─────────────────────►│
     │                    │                      │
     │  ◄─────────────────┤   Generate Tokens    │
     │  { accessToken,    │◄─────────────────────┤
     │    refreshToken }  │                      │
```

### Token Refresh Flow:
```
┌─────────┐         ┌────────────┐        ┌──────────┐
│ Client  │────────►│   API      │───────►│ Database │
└─────────┘         └────────────┘        └──────────┘
     │                    │                      │
     │ POST /refresh      │  Validate Refresh    │
     ├───────────────────►│─────────────────────►│
     │                    │                      │
     │  ◄─────────────────┤   New Tokens         │
     │  { new tokens }    │◄─────────────────────┤
```

---

## 🚀 API Endpoints Summary

### Public Endpoints (No Auth Required):
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh-token` - Refresh access token

### Protected Endpoints (Auth Required):
- `POST /api/auth/revoke-token` - Logout/revoke tokens  
- `POST /api/auth/change-password` - Change password
- `GET /api/auth/me` - Get current user info
- `GET /api/users/{id}` - Get user (self or admin)

### Admin-Only Endpoints:
- `GET /api/roles` - List all roles
- `POST /api/roles` - Create role
- `PUT /api/roles/{id}` - Update role
- `DELETE /api/roles/{id}` - Delete role
- `POST /api/roles/assign` - Assign role to user
- `GET /api/users` - List all users
- `PUT /api/users/{id}/activate` - Activate user
- `PUT /api/users/{id}/deactivate` - Deactivate user
- `DELETE /api/users/{id}` - Delete user

---

## 📊 Database Schema Updates

### New Table: RefreshTokens
```sql
CREATE TABLE RefreshTokens (
    Id INT PRIMARY KEY IDENTITY,
    UserId NVARCHAR(450) NOT NULL,
    Token NVARCHAR(MAX) NOT NULL,
    JwtId NVARCHAR(MAX) NOT NULL,
    IsUsed BIT NOT NULL,
    IsRevoked BIT NOT NULL,
    CreatedDate DATETIME2 NOT NULL,
    ExpiryDate DATETIME2 NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
)
```

### Updated Seed Data:
- 6 predefined roles (see Features section)
- Default admin user retained

---

## 🧪 Testing the API

### Using Swagger:
1. Navigate to: `https://localhost:5001/swagger`
2. Click "Authorize" button
3. Enter: `Bearer {your_token}`
4. Test endpoints directly

### Using Postman/Thunder Client:
1. Import the API endpoints
2. Set Authorization header: `Bearer {token}`
3. Test all CRUD operations

### Sample cURL Command:
```bash
# Login
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"userName":"admin","password":"Admin@123"}'

# Get current user
curl -X GET https://localhost:5001/api/auth/me \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

---

## 🔒 Security Considerations

### Implemented:
- ✅ JWT token expiration  
- ✅ Refresh token mechanism
- ✅ Token revocation on logout
- ✅ Role-based access control
- ✅ Password validation
- ✅ IsUsed/IsRevoked flags for refresh tokens

### Production Recommendations:
- 🔹 Implement BCrypt for password hashing
- 🔹 Add rate limiting for login attempts
- 🔹 Implement CORS policies
- 🔹 Use HTTPS only
- 🔹 Add email verification
- 🔹 Implement 2FA (optional)
- 🔹 Add audit logging
- 🔹 Monitor failed authentication attempts

---

## 📖 Documentation

### Complete API Documentation:
See `API_DOCUMENTATION.md` for:
- Detailed endpoint descriptions
- Request/response examples
- Error handling
- Security best practices
- Code examples (cURL, JavaScript)

---

## ✨ Next Steps

### Immediate:
1. Test all endpoints using Swagger
2. Verify role-based authorization
3. Test token refresh flow  
4. Validate password change functionality

### Future Enhancements:
1. Implement BCrypt password hashing
2. Add email verification system
3. Implement forgot password flow
4. Add 2FA support
5. Create admin dashboard for user management
6. Add audit/activity logging
7. Implement rate limiting middleware

---

## 💡 Example Usage in Blazor

```csharp
// In your Blazor component
@inject HttpClient Http

private async Task LoginAndCallProtectedEndpoint()
{
    // Login
    var loginRequest = new { userName = "admin", password = "Admin@123" };
    var loginResponse = await Http.PostAsJsonAsync("api/auth/login", loginRequest);
    var authResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
    
    if (authResult.Success)
    {
        // Store token
        var token = authResult.AccessToken;
        
        // Call protected endpoint
        Http.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        var me = await Http.GetFromJsonAsync<object>("api/auth/me");
    }
}
```

---

## 🎉 Conclusion

The JWT Authorization API has been successfully integrated with:
- ✅ Complete authentication system
- ✅ Role-based authorization
- ✅ Refresh token mechanism
- ✅ Comprehensive API endpoints
- ✅ Full documentation
- ✅ Database migrations
- ✅ Security best practices

The system is ready for development and testing. For production deployment, follow the security recommendations listed above.

---

## 📞 Support

For questions or issues regarding the JWT implementation:
1. Review `API_DOCUMENTATION.md`
2. Check Swagger documentation at `/swagger`
3. Contact the development team

**Implementation Date:** December 27, 2025  
**Version:** 1.0.0  
**.NET Version:** 8.0

---

# JWT Quick Reference

## 🚀 Quick Start

### 1. Login (Get JWT Token)
```bash
POST /api/auth/login
{
  "userName": "admin",
  "password": "Admin@123"
}

# Response includes accessToken and refreshToken
```

### 2. Use Token in Requests
```bash
Authorization: Bearer YOUR_ACCESS_TOKEN_HERE
```

### 3. Refresh Expired Token
```bash
POST /api/auth/refresh-token
{
  "accessToken": "expired_token",
  "refreshToken": "valid_refresh_token"
}
```

---

## 🔑 Available Roles

| Role ID | Role Name | Description |
|---------|-----------|-------------|
| 1 | Administrator | Full access |
| 2 | Inventory Admin | Full inventory management |
| 3 | Inventory Clerk | Inventory operations |
| 4 | Inventory Reports Viewer | View only |
| 5 | Sales Clerk | Sales operations |
| 6 | General User | Basic access |

---

## 📡 Main Endpoints

### Authentication
- `POST /api/auth/register` - Register
- `POST /api/auth/login` - Login
- `POST /api/auth/refresh-token` - Refresh
- `POST /api/auth/revoke-token` 🔒 - Logout
- `POST /api/auth/change-password` 🔒 - Change pwd
- `GET /api/auth/me` 🔒 - Current user

### Roles (👑 Admin Only)
- `GET /api/roles` - List roles
- `POST /api/roles` - Create role
- `PUT /api/roles/{id}` - Update
- `DELETE /api/roles/{id}` - Delete
- `POST /api/roles/assign` - Assign to user

### Users
- `GET /api/users` 👑 - List all
- `GET /api/users/{id}` 🔒 - Get user
- `PUT /api/users/{id}/activate` 👑 - Activate
- `PUT /api/users/{id}/deactivate` 👑 - Deactivate
- `DELETE /api/users/{id}` 👑 - Delete

🔒 = Auth Required  
👑 = Admin Only

---

## ⚡ Example Requests

### Register
```json
POST /api/auth/register
{
  "userName": "newuser",
  "email": "user@example.com",
  "password": "Password123",
  "confirmPassword": "Password123",
  "roleId": "6"
}
```

### Login
```json
POST /api/auth/login
{
  "userName": "admin",
  "password": "Admin@123"
}
```

### Create Role (Admin)
```json
POST /api/roles
Authorization: Bearer {admin_token}
{
  "name": "Custom Role",
  "description": "Description",
  "isAdministrative": false,
  "canEditModel": true
}
```

### Assign Role (Admin)
```json
POST /api/roles/assign
Authorization: Bearer {admin_token}
{
  "userId": "user-id-here",
  "roleId": "2"
}
```

---

## 🔐 Token Info

- **Access Token:** 60 minutes
- **Refresh Token:** 7 days
- **Storage:** Database (can be revoked)

---

## 🧪 Test in Swagger

1. Run: `dotnet run`
2. Open: `https://localhost:5001/swagger`
3. Click "Authorize"
4. Enter: `Bearer {your_token}`
5. Test endpoints!

---

## 📱 JavaScript Example

```javascript
// Login
const response = await fetch('/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    userName: 'admin',
    password: 'Admin@123'
  })
});

const { accessToken, refreshToken } = await response.json();

// Use token
const userResponse = await fetch('/api/auth/me', {
  headers: { 'Authorization': `Bearer ${accessToken}` }
});
```

---

For complete documentation, see `API_DOCUMENTATION.md`

---
*Last Updated: December 2025*