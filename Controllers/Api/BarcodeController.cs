using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EyeHospitalPOS.Services;
using EyeHospitalPOS.Models;
using EyeHospitalPOS.Interfaces;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BarcodeController : ControllerBase
    {
        private readonly IBarcodeService _barcodeService;
        private readonly IProductService _productService;

        public BarcodeController(IBarcodeService barcodeService, IProductService productService)
        {
            _barcodeService = barcodeService;
            _productService = productService;
        }

        [HttpPost("decode-barcode-image")]
        public async Task<ActionResult<BarcodeDecodeResponse>> DecodeBarcodeImage([FromBody] BarcodeImageRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.ImageData))
                {
                    return BadRequest(new BarcodeDecodeResponse
                    {
                        Success = false,
                        ErrorMessage = "Image data is required"
                    });
                }

                // Decode the barcode from the image
                var decodeResult = _barcodeService.DecodeFromBase64(request.ImageData);

                if (!decodeResult.Success)
                {
                    return Ok(new BarcodeDecodeResponse
                    {
                        Success = false,
                        ErrorMessage = decodeResult.ErrorMessage
                    });
                }

                // If barcode was successfully decoded, try to find the associated product
                Product? product = null;
                if (!string.IsNullOrEmpty(decodeResult.Barcode))
                {
                    product = await _productService.GetProductByBarcodeAsync(decodeResult.Barcode);
                }

                return Ok(new BarcodeDecodeResponse
                {
                    Success = true,
                    Barcode = decodeResult.Barcode,
                    Format = decodeResult.Format,
                    Product = product
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new BarcodeDecodeResponse
                {
                    Success = false,
                    ErrorMessage = $"Error processing barcode image: {ex.Message}"
                });
            }
        }
    }
}