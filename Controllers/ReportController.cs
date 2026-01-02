using System.Threading.Tasks;
using EyeHospitalPOS.Services;
using Microsoft.AspNetCore.Mvc;

namespace EyeHospitalPOS.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ReportGenerator _reportGenerator;

        public ReportController(ReportGenerator reportGenerator)
        {
            _reportGenerator = reportGenerator;
        }

        [HttpGet("sales-invoice/{id}")]
        public async Task<IActionResult> GetSalesInvoice(int id)
        {
            try
            {
                var pdfBytes = await _reportGenerator.GenerateInvoicePdfAsync(id);
                
                if (pdfBytes == null)
                {
                    return NotFound("Invoice not found or could not be generated.");
                }

                var filename = $"Invoice-{id}.pdf";
                return File(pdfBytes, "application/pdf", filename);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error generating report: {ex.Message}");
            }
        }
    }
}
