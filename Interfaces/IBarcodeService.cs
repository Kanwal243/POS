using ZXing;

namespace EyeHospitalPOS.Interfaces
{
    public interface IBarcodeService
    {
        bool ValidateBarcodeText(string barcodeText);
        BarcodeFormat GetSuggestedFormat(string barcodeText);
        BarcodeDecodeResult DecodeFromBase64(string base64Image);
        BarcodeDecodeResult DecodeFromBytes(byte[] imageBytes);
    }

    public class BarcodeDecodeResult
    {
        public bool Success { get; set; }
        public string? Barcode { get; set; }
        public string? Format { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
