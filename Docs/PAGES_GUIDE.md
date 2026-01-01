# Eye Hospital POS - Page-by-Page Feature Guide

This guide provides a detailed breakdown of the functionality available on every page of the application.

---

## 🚀 Dashboard (`/dashboard`)
The central command hub for a quick business overview.
*   **Total Customers Card**: Displays total registered patients/customers with a growth trend percentage.
*   **Total Orders Card**: Summarizes the total sales orders processed.
*   **Dynamic Currency Support**: All financial figures automatically adapt to the organization's currency symbol.
*   **Real-time Analytics**: (Infrastructure for) Top selling products and customer performance charts.
 ---

## 📦 Products Catalog (`/product`)
The master registry of all inventory items.
*   **Premium Data Grid**: Filterable, searchable, and paginated list of all products.
*   **Inventory Status**: Visual badges indicating "Active" or "Inactive" status.
*   **Dual Scanning Modes**:
    *   **External Scanner**: Modal interface to quickly look up product details via barcode.
    *   **Form Auto-fill**: Inline scanner support within the Add/Edit form to populate product data instantly.
*   **Pricing Management**: Input fields for Cost Price and Sale Price with dynamic currency formatting.
*   **Stock Monitoring**: Integrated stock quantity and reorder level indicators.

---

## 🚛 Inventory Receiving (`/inventory-receiving`)
Tracks the arrival of goods from suppliers.
*   **Batch Processing**: Record bulk arrivals of products into the system.
*   **Detailed IR Form**:
    *   Supplier selection.
    *   Transaction date and reference number management.
    *   Line-item entry with unit cost and subtotal calculations.
*   **Status Management**: Distinguishes between "Draft" arrivals and "Active" confirmed stock-ins.
*   **Automatic Stock Update**: Confirming an IR record instantly increments the product warehouse quantities.

---

## 📑 Purchase Orders (`/purchase-order`)
The procurement management module.
*   **Procurement Workflow**: Management of orders from Draft to Active, and eventually Completed or Cancelled.
*   **PO Lifecycle**:
    *   Drafting with product selection and cost negotiation.
    *   Activation to finalize order requirements.
    *   Generating an Inventory Receiving (IR) record directly from an active PO to minimize manual entry.
*   **Summary Footer**: Real-time total items count and total payable amount calculations.

---

## 🧾 Sales & Invoices (`/invoices`)
The historical registry of customer billing.
*   **Audit Trail**: Searchable list of all generated invoices with unique `InvoiceNumber`.
*   **Customer Linking**: Direct visibility into which customer/patient was billed for each transaction.
*   **Status Tracking**: Indicators for "Printed" vs "Draft" invoices.
*   **Financial Visibility**: Clear display of total sale amounts with dynamic currency support.

---

## 👥 Customer & Supplier Management
### Customers (`/customers`)
*   **Patient Registry**: Management of name, phone, email, and display names.
*   **Status Control**: Toggle customers between Active and Inactive.
*   **Quick Search**: Find customers by phone or name for rapid POS checkout.

### Suppliers (`/suppliers`)
*   **Vendor Management**: Track contact details, addresses, and status of inventory providers.
*   **Supplier Linking**: Associated with Products and Purchase Orders for streamlined procurement.

---

## ⚙️ System Setup & Security
### Organization Info (`/info`)
*   **White-labeling**: Configure hospital name, logo, phone, and address.
*   **Global Localization**: The **sole source of truth** for:
    *   Base Currency (e.g., USD, PKR).
    *   Date Display Format.
    *   Local Time Zone.

### User Management (`/users`)
*   **Account Controls**: Create, update, and manage system users.
*   **Security Protocol**: Toggle "Change Password on Login" for new staff members.

### Role Management (`/roles`)
*   **Granular RBAC**: Assign page-level visibility and action-level permissions (CanEdit, CanAdd, etc.).
*   **Hierarchy**: Supports parent-child role relations for complex organizational structures.

---

## 🔐 Personal Settings
### Profile (`/profile`)
*   **Self-Service**: Users can update their personal information and contact details.
*   **Session Info**: Displays assigned roles and system permissions.

### Change Password (`/change-password`)
*   **Security Check**: Self-service password updates with validation checks.

---
*Last Updated: December 2025*

