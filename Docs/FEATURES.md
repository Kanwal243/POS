# Eye Hospital POS - Application Features

A comprehensive list of features implemented in the Eye Hospital Point of Sale (POS) system.

## 1. Core POS & Sales Management
*   **Touch-Friendly POS Interface**: Optimized screen for rapid transaction processing.
*   **Barcode Scanning**: Integrated search and auto-entry using barcode scanners.
*   **Cart Management**: Add/remove items, dynamic quantity adjustment, and stock-level validation.
*   **Discounting System**: Support for percentage-based discounts on the total payable amount.
*   **Multiple Payment Methods**: Support for Cash, Card, UPI, and Bank Transfer.
*   **Invoice Generation**: Automated generation of unique invoice numbers upon sale completion.
*   **Invoice History**: Dedicated module to search, view, and reprint previous transactions.
*   **Customer Management**: Profiles for walk-in customers and registered patients.

## 2. Advanced Inventory & Procurement
*   **Product Catalog**: Robust management of frames, lenses, medicines, and services.
*   **Multi-Attribute Products**: Support for Category, Product Type, and Supplier associations.
*   **Stock Tracking**: Real-time monitoring of stock levels with visual alerts for low inventory.
*   **Purchase Orders (PO)**: Complete procurement lifecycle management (Draft -> Active -> Completed/Cancelled).
*   **Inventory Receiving (IR)**: Formalized stock-in process to update inventory levels, linked to POs or standalone.
*   **Supplier Management**: Comprehensive registry of vendors and their contact information.
*   **Reorder Alerts**: Configurable reorder levels for each product to prevent stockouts.

## 3. System Administration & Security
*   **Regional Settings**: Dynamic currency selection (USD, PKR, EUR, etc.) that reflects throughout the app.
*   **Organization Profile**: Centralized hospital identity management (Logo, Name, Address, Contact).
*   **Role-Based Access Control (RBAC)**:
    *   **Page-Level Permissions**: Control who can see specific modules.
    *   **Action-Level Permissions**: Manage Add/Edit/Delete capabilities per role.
    *   **Role Hierarchy**: Support for parent-child role structures.
*   **JWT Authentication**: Secure, industrial-standard login with access and refresh token logic.
*   **Security Compliance**: Required password changes for new users on their first login.
*   **User Management**: Full CRUD operations for system users with status (Active/Inactive) control.

## 4. Reporting & Analytics
*   **Interactive Dashboard**: Real-time visual metrics including:
    *   Total Sales Revenue
    *   Total Order Volume
    *   Customer Growth Trends
*   **Top Performance Insights**: Identification of top-selling products and highest-value customers.
*   **Category Sales**: Breakdown of revenue across different product departments.
*   **Audit Logs**: (Infrastructure ready) Tracking of significant system changes.

## 5. Technical Highlights
*   **Modern Technology Stack**: Built with .NET 8.0 and Blazor Server.
*   **DevExpress UI**: Premium, high-performance UI components for data grids and forms.
*   **Scalable Architecture**: Interface-driven service layer (SOLID principles) for easy maintenance.
*   **Responsive Design**: Modern, premium CSS theme with sidebar navigation and mobile-friendly layouts.
*   **Hardware Ready**: Built-in support for barcode scanners and printer-ready invoice templates.

---
*Last Updated: December 2025*

