using System;

namespace EyeHospitalPOS.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public int SaleId { get; set; }
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        public string? PdfPath { get; set; }
        public bool IsPrinted { get; set; } = false;

        // Navigation
        public Sale? Sale { get; set; }
    }
}
