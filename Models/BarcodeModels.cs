using System.Collections.Generic;

namespace EyeHospitalPOS.Models
{
    /// <summary>
    /// Request model for single barcode validation
    /// </summary>
    public class BarcodeValidationRequest
    {
        public string Barcode { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response model for barcode validation
    /// </summary>
    public class BarcodeValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int StockQuantity { get; set; }
        public decimal SalePrice { get; set; }
    }

    /// <summary>
    /// Request model for barcode search
    /// </summary>
    public class BarcodeSearchRequest
    {
        public List<string> Barcodes { get; set; } = new List<string>();
    }

    /// <summary>
    /// Request model for bulk barcode validation
    /// </summary>
    public class BulkBarcodeValidationRequest
    {
        public List<string> Barcodes { get; set; } = new List<string>();
    }

    /// <summary>
    /// Response model for bulk barcode validation
    /// </summary>
    public class BulkBarcodeValidationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ValidCount { get; set; }
        public int InvalidCount { get; set; }
        public List<BarcodeValidationResult> ValidBarcodes { get; set; } = new List<BarcodeValidationResult>();
        public List<BarcodeValidationResult> InvalidBarcodes { get; set; } = new List<BarcodeValidationResult>();
    }

    /// <summary>
    /// Model for bulk product import from barcodes
    /// </summary>
    public class BulkProductImportRequest
    {
        public List<ProductImportItem> Products { get; set; } = new List<ProductImportItem>();
        public bool SkipDuplicates { get; set; } = true;
    }

    /// <summary>
    /// Individual product item for import
    /// </summary>
    public class ProductImportItem
    {
        public string Name { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int TypeId { get; set; }
        public int SupplierId { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; } = 10;
    }

    /// <summary>
    /// Response model for bulk import
    /// </summary>
    public class BulkImportResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ImportedCount { get; set; }
        public int SkippedCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<Product> ImportedProducts { get; set; } = new List<Product>();
    }

    /// <summary>
    /// Model for barcode printing request
    /// </summary>
    public class BarcodePrintRequest
    {
        public List<int> ProductIds { get; set; } = new List<int>();
        public string Format { get; set; } = "CODE128"; // CODE128, EAN13, UPC, QR
        public int Quantity { get; set; } = 1; // How many copies of each barcode
        public int Width { get; set; } = 2; // Barcode width in mm
        public int Height { get; set; } = 40; // Barcode height in mm
    }

    /// <summary>
    /// Response model for barcode print job
    /// </summary>
    public class BarcodePrintResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string PrintJobId { get; set; } = string.Empty;
        public int BarcodeCount { get; set; }
        public string PdfUrl { get; set; } = string.Empty; // URL to download generated PDF
    }

    /// <summary>
    /// Model for camera scanning callback
    /// </summary>
    public class CameraScanResult
    {
        public string Barcode { get; set; } = string.Empty;
        public long ScanTimestamp { get; set; }
        public string Format { get; set; } = string.Empty; // CODE128, QR, EAN, etc.
    }

    /// <summary>
    /// Response for camera scan processing
    /// </summary>
    public class CameraScanResponse
    {
        public bool IsProcessed { get; set; }
        public string Message { get; set; } = string.Empty;
        public Product? Product { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Request model for barcode image decoding
    /// </summary>
    public class BarcodeImageRequest
    {
        public string ImageData { get; set; } = string.Empty; // Base64 encoded image
    }

    /// <summary>
    /// Response model for barcode image decoding
    /// </summary>
    public class BarcodeDecodeResponse
    {
        public bool Success { get; set; }
        public string? Barcode { get; set; }
        public string? Format { get; set; }
        public string? ErrorMessage { get; set; }
        public Product? Product { get; set; }
    }
}
