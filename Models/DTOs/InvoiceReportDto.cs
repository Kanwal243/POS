using System;
using System.Collections.Generic;

namespace EyeHospitalPOS.Models.DTOs
{
    public class InvoiceReportDto
    {
        public string Title { get; set; } = "Sales Invoice";
        public string OrganizationName { get; set; } = "";
        public string OrganizationAddress { get; set; } = "";
        public string OrganizationPhone { get; set; } = "";
        public string OrganizationEmail { get; set; } = "";

        public string InvoiceNumber { get; set; } = "";
        public string Date { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string PaymentMode { get; set; } = "";
        public string UserName { get; set; } = "";

        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        
        public List<InvoiceItemDto> Items { get; set; } = new List<InvoiceItemDto>();
    }

    public class InvoiceItemDto
    {
        public int Sr { get; set; }
        public string Description { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public decimal Payable { get; set; }
    }
}
