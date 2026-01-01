# Eye Hospital POS - Application Workflow

This document outlines the core business workflows and technical architecture of the Eye Hospital Point of Sale (POS) system.

## 1. System Initialization & Setup
The foundation of the application configuration.

*   **Organization Configuration**: Set hospital name, contact details, and localization settings (Currency, Date Format, Time Zone) in `System Setup > Organization Info`.
*   **Access Control**: 
    *   **Roles & Permissions**: Define roles (e.g., Administrator, Sales Clerk, Inventory Admin) and assign granular permissions for pages and modules.
    *   **User Management**: Create system users and assign them specific roles.
*   **Security**: Enforce password policies, including mandatory password changes on first login for new users.

## 2. Inventory Management Workflow
The cycle of managing ophthalmic products and stock levels.

### A. Product Catalog Setup
1.  Define **Product Categories** (e.g., Frames, Lenses, Medicine).
2.  Define **Product Types** (e.g., Single Vision, Bifocal).
3.  Register **Suppliers** who provide the inventory.

### B. Procurement (Purchase Orders)
1.  **Draft**: Create a new Purchase Order (PO) by selecting a supplier and adding products.
2.  **Activation**: Review and activate the PO to authorize the order.
3.  **Completion/IR**: Once items arrive, generate an **Inventory Receiving (IR)** record from the PO.

### C. Inventory Receiving (Stock In)
*   **Direct Receiving**: Stock can also be received without a PO.
*   **Stock Update**: Activating an Inventory Receiving record automatically increments the `StockQuantity` of the associated products in the database.

## 3. Sales & Billing Workflow
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

## 4. Technical Architecture & Services
The application follows a clean, interface-driven service architecture.

*   **Authentication**: Managed via `IAuthService` and JWT tokens. Handles login, token refresh, and user state.
*   **Hardware Integration**: `IBarcodeService` handles barcode generation and decoding using ZXing.
*   **Inventory Logic**: `IInventoryService` and `IPurchaseOrderService` manage complex stock transactions and state transitions.
*   **Identity Context**: `IHttpContextService` provides abstracted access to the current user's identity and claims throughout the service layer.
*   **UI Components**: Utilizes **DevExpress Blazor Components** for high-performance grids, editors, and popups.

## 5. Reporting & Analytics
*   **Dashboard**: Real-time summary of total sales, order counts, top products, and customer trends.
*   **Reports**: Detailed modules for auditing inventory levels, sales performance, and system logs.

---
*Last Updated: December 2025*

