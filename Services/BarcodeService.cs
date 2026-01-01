using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using System.Drawing;
using System.Drawing.Imaging;
using EyeHospitalPOS.Interfaces;

namespace EyeHospitalPOS.Services
{
    public class BarcodeService : IBarcodeService
    {
        private readonly DecodingOptions _decodingOptions;

        public BarcodeService()
        {
            // Configure ZXing decoding options with multiple format support
            _decodingOptions = new DecodingOptions
            {
                PossibleFormats = new List<BarcodeFormat>
                {
                    BarcodeFormat.CODE_128,
                    BarcodeFormat.EAN_13,
                    BarcodeFormat.EAN_8,
                    BarcodeFormat.UPC_A,
                    BarcodeFormat.UPC_E,
                    BarcodeFormat.CODE_39,
                    BarcodeFormat.QR_CODE,
                    BarcodeFormat.DATA_MATRIX,
                    BarcodeFormat.PDF_417
                },
                TryHarder = true,
                PureBarcode = false
            };
        }

        /// <summary>
        /// Validates if the barcode text is in a valid format
        /// </summary>
        public bool ValidateBarcodeText(string barcodeText)
        {
            return !string.IsNullOrWhiteSpace(barcodeText) && barcodeText.Length > 0;
        }

        /// <summary>
        /// Gets the suggested barcode format based on barcode text
        /// </summary>
        public BarcodeFormat GetSuggestedFormat(string barcodeText)
        {
            if (string.IsNullOrWhiteSpace(barcodeText))
                return BarcodeFormat.CODE_128;

            // EAN-13: 13 digits
            if (barcodeText.Length == 13 && barcodeText.All(char.IsDigit))
                return BarcodeFormat.EAN_13;

            // EAN-8: 8 digits
            if (barcodeText.Length == 8 && barcodeText.All(char.IsDigit))
                return BarcodeFormat.EAN_8;

            // UPC-A: 12 digits
            if (barcodeText.Length == 12 && barcodeText.All(char.IsDigit))
                return BarcodeFormat.UPC_A;

            // Default to CODE_128 for alphanumeric
            return BarcodeFormat.CODE_128;
        }

        /// <summary>
        /// Decodes barcode from base64 image data
        /// </summary>
        public BarcodeDecodeResult DecodeFromBase64(string base64Image)
        {
            var result = new BarcodeDecodeResult();

            try
            {
                if (string.IsNullOrWhiteSpace(base64Image))
                {
                    result.Success = false;
                    result.ErrorMessage = "Image data is empty";
                    return result;
                }

                // Remove data URL prefix if present (data:image/jpeg;base64,...)
                string base64Data = base64Image;
                if (base64Image.Contains(","))
                {
                    base64Data = base64Image.Split(',')[1];
                }

                // Convert base64 to byte array
                byte[] imageBytes = Convert.FromBase64String(base64Data);

                return DecodeFromBytes(imageBytes);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Error decoding barcode: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// Decodes barcode from byte array
        /// </summary>
        public BarcodeDecodeResult DecodeFromBytes(byte[] imageBytes)
        {
            var result = new BarcodeDecodeResult();

            try
            {
                if (imageBytes == null || imageBytes.Length == 0)
                {
                    result.Success = false;
                    result.ErrorMessage = "Image data is empty";
                    return result;
                }

                using (var ms = new MemoryStream(imageBytes))
                using (var bitmap = new Bitmap(ms))
                {
                    // Lock the bits of the bitmap
                    var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                    var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

                    try
                    {
                        var length = Math.Abs(data.Stride) * bitmap.Height;
                        var bytes = new byte[length];
                        System.Runtime.InteropServices.Marshal.Copy(data.Scan0, bytes, 0, length);

                        // Create generic luminance source (RGBLuminanceSource is standard in ZXing core)
                        var luminanceSource = new RGBLuminanceSource(bytes, bitmap.Width, bitmap.Height, RGBLuminanceSource.BitmapFormat.RGB32);
                        var binarizer = new HybridBinarizer(luminanceSource);
                        var binaryBitmap = new BinaryBitmap(binarizer);

                        // Use MultiFormatReader directly
                        var reader = new MultiFormatReader();
                        // Note: DecodingOptions might have a Hints property or we pass it manually
                        var decodeResult = reader.decode(binaryBitmap, _decodingOptions.Hints);

                        if (decodeResult != null && !string.IsNullOrEmpty(decodeResult.Text))
                        {
                            result.Success = true;
                            result.Barcode = decodeResult.Text;
                            result.Format = decodeResult.BarcodeFormat.ToString();
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "No barcode detected in image";
                        }
                    }
                    finally
                    {
                        bitmap.UnlockBits(data);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Error decoding barcode: {ex.Message}";
            }

            return result;
        }
    }
}

