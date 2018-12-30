using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using BarcodeLib;
using Known.Drawing;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;

namespace Known.Tests.Providers
{
    public class BarCodeProvider : IBarCodeProvider
    {
        public Bitmap CreateBarCode(string content)
        {
            var barcode = new Barcode();
            barcode.BackColor = Color.Transparent;
            barcode.ForeColor = Color.Black;
            barcode.IncludeLabel = true;
            barcode.Alignment = AlignmentPositions.CENTER;
            barcode.LabelPosition = LabelPositions.BOTTOMCENTER;
            barcode.ImageFormat = ImageFormat.Png;
            var image = barcode.Encode(TYPE.CODE39, content);
            return new Bitmap(image);
        }

        public string GetBarCodeContent(Bitmap bitmap)
        {
            throw new NotImplementedException();
        }

        public Bitmap CreateQrCode(string content)
        {
            var encoder = new QRCodeEncoder
            {
                QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE,
                QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H,
                QRCodeScale = 3,
                QRCodeVersion = 0
            };
            return encoder.Encode(content, Encoding.UTF8);
        }

        public string GetQrCodeContent(Bitmap bitmap)
        {
            var decoder = new QRCodeDecoder();
            return decoder.decode(new QRCodeBitmapImage(bitmap));
        }
    }
}
