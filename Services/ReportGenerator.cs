using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EyeHospitalPOS.Data;
using EyeHospitalPOS.Models.DTOs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using FastReport;
using FastReport.Export.PdfSimple;
using FastReport.Utils;
using System.Drawing;

namespace EyeHospitalPOS.Services
{
    public class ReportGenerator
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ReportGenerator(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(int saleId)
        {
            // 1. Fetch Data
            var sale = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.User)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null) return null;

            var org = await _context.Organizations.FirstOrDefaultAsync();

            // 2. Map to DTO
            var dto = new InvoiceReportDto
            {
                OrganizationName = org?.Name ?? "Eye Hospital",
                OrganizationAddress = org?.Address ?? "",
                OrganizationPhone = org?.Phone ?? "",
                OrganizationEmail = org?.Email ?? "",
                
                InvoiceNumber = sale.InvoiceNumber ?? $"INV-{sale.Id}",
                Date = sale.SaleDate.ToString("dd-MMM-yyyy"),
                CustomerName = sale.Customer?.DisplayName ?? "Walk-in",
                PaymentMode = sale.PaymentMode.ToString(),
                UserName = sale.User?.UserName ?? "Admin",
                
                SubTotal = sale.SubTotal,
                Discount = sale.DiscountAmount,
                TotalAmount = sale.TotalAmount,
                
                Items = sale.SaleItems.Select((si, index) => new InvoiceItemDto
                {
                    Sr = index + 1,
                    Description = si.Product?.Name ?? "Item",
                    Quantity = si.Quantity,
                    Rate = si.UnitPrice,
                    Amount = si.Quantity * si.UnitPrice,
                    Discount = si.Discount,
                    Payable = si.Subtotal // Using Subtotal as payable per item (after discount)
                }).ToList()
            };

            // 3. Create Report Programmatically
            using var report = new Report();
            
            // Register Data using Flattened Lists to simplify binding
            report.RegisterData(dto.Items, "Items");
            report.RegisterData(new[] { dto }, "Invoice");
            
            report.GetDataSource("Items").Enabled = true;
            report.GetDataSource("Invoice").Enabled = true;

            // Page Setup
            ReportPage page = new ReportPage();
            page.Name = "Page1";
            report.Pages.Add(page);
            page.LeftMargin = 10; 
            page.RightMargin = 10; 
            page.TopMargin = 10; 
            page.BottomMargin = 10; // mm

            // Report Title Band
            ReportTitleBand titleBand = new ReportTitleBand();
            titleBand.Name = "ReportTitle";
            titleBand.Height = Units.Centimeters * 4.0f;
            page.ReportTitle = titleBand;

            // Organization Name
            CreateText(titleBand, dto.OrganizationName, 0, 0, 19, 1, 16, FontStyle.Bold, HorzAlign.Center);
            
            // Address & Contact
            string contactInfo = $"{dto.OrganizationAddress}\nPhone: {dto.OrganizationPhone} | Email: {dto.OrganizationEmail}";
            CreateText(titleBand, contactInfo, 0, 1.0f, 19, 1.2f, 9, FontStyle.Regular, HorzAlign.Center);

            // Invoice Details Box
            var invoiceInfoText = $"Invoice No: {dto.InvoiceNumber}                Date: {dto.Date}\nCustomer: {dto.CustomerName} ({dto.PaymentMode})";
            var invObj = CreateText(titleBand, invoiceInfoText, 0, 2.5f, 19, 1.2f, 10, FontStyle.Bold, HorzAlign.Left);
            invObj.Border.Lines = BorderLines.Top | BorderLines.Bottom;


            // Column Header Band
            DataHeaderBand headerBand = new DataHeaderBand();
            headerBand.Name = "Header";
            headerBand.Height = Units.Centimeters * 0.8f;
            page.Bands.Add(headerBand);

            float[] colWidths = new float[] { 1.5f, 8.0f, 2.0f, 2.5f, 2.5f, 2.5f }; // cm
            string[] colNames = new string[] { "Sr", "Description", "Qty", "Rate", "Discount", "Amount" };
            float currentLeft = 0;

            for (int i = 0; i < colNames.Length; i++)
            {
                var text = CreateText(headerBand, colNames[i], currentLeft, 0, colWidths[i], 0.8f, 9, FontStyle.Bold, HorzAlign.Center);
                text.Border.Lines = BorderLines.Bottom;
                text.FillColor = Color.LightGray;
                currentLeft += colWidths[i];
            }

            // Data Band
            DataBand dataBand = new DataBand();
            dataBand.Name = "Data";
            dataBand.DataSource = report.GetDataSource("Items");
            dataBand.Height = Units.Centimeters * 0.6f;
            page.Bands.Add(dataBand);

            currentLeft = 0;
            string[] dataFields = new string[] { "[Items.Sr]", "[Items.Description]", "[Items.Quantity]", "[Items.Rate]", "[Items.Discount]", "[Items.Payable]" };
            HorzAlign[] aligns = new HorzAlign[] { HorzAlign.Center, HorzAlign.Left, HorzAlign.Center, HorzAlign.Right, HorzAlign.Right, HorzAlign.Right };
            string[] formats = new string[] { "", "", "", "N2", "N2", "N2" };

            for (int i = 0; i < dataFields.Length; i++)
            {
                var text = CreateText(dataBand, dataFields[i], currentLeft, 0, colWidths[i], 0.6f, 9, FontStyle.Regular, aligns[i]);
                if (formats[i] == "N2") 
                {
                    text.Format = new FastReport.Format.NumberFormat { DecimalDigits = 2, UseLocale = false, DecimalSeparator = ".", GroupSeparator = "," };
                }
                else
                {
                    text.Format = null;
                }
                currentLeft += colWidths[i];
            }

            // Footer Band (Totals)
            DataFooterBand footerBand = new DataFooterBand();
            footerBand.Name = "Footer";
            footerBand.Height = Units.Centimeters * 2.5f;
            page.Bands.Add(footerBand);

            // Total Line
            var lineObj = new LineObject();
            lineObj.Parent = footerBand;
            lineObj.Left = 0;
            lineObj.Width = Units.Centimeters * 19;
            lineObj.Top = 0;

            // Totals
            CreateText(footerBand, "Sub Total:", 12, 0.2f, 4, 0.6f, 10, FontStyle.Bold, HorzAlign.Right);
            var subTotalObj = CreateText(footerBand, $"{dto.SubTotal:N2}", 16, 0.2f, 3, 0.6f, 10, FontStyle.Bold, HorzAlign.Right);

            CreateText(footerBand, "Discount:", 12, 0.8f, 4, 0.6f, 10, FontStyle.Bold, HorzAlign.Right);
            CreateText(footerBand, $"{dto.Discount:N2}", 16, 0.8f, 3, 0.6f, 10, FontStyle.Bold, HorzAlign.Right);

            CreateText(footerBand, "Grand Total:", 12, 1.4f, 4, 0.6f, 11, FontStyle.Bold, HorzAlign.Right);
            var totalObj = CreateText(footerBand, $"{dto.TotalAmount:N2}", 16, 1.4f, 3, 0.6f, 11, FontStyle.Bold, HorzAlign.Right);
            totalObj.Border.Lines = BorderLines.Top | BorderLines.Bottom;

            // Signature
            CreateText(footerBand, "Issued By: " + dto.UserName, 0, 1.5f, 8, 0.6f, 9, FontStyle.Italic, HorzAlign.Left);


            // 4. Prepare and Export
            if (report.Prepare())
            {
                using var ms = new MemoryStream();
                var pdfExport = new PDFSimpleExport();
                report.Export(pdfExport, ms);
                return ms.ToArray();
            }

            return null;
        }

        private TextObject CreateText(BandBase band, string text, float leftCm, float topCm, float widthCm, float heightCm, float fontSize, FontStyle fontStyle, HorzAlign align)
        {
            TextObject textObj = new TextObject();
            textObj.Parent = band;
            textObj.Text = text;
            textObj.Bounds = new RectangleF(Units.Centimeters * leftCm, Units.Centimeters * topCm, Units.Centimeters * widthCm, Units.Centimeters * heightCm);
            textObj.Font = new Font("Arial", fontSize, fontStyle);
            textObj.HorzAlign = align;
            textObj.VertAlign = VertAlign.Center;
            return textObj;
        }
    }
}
