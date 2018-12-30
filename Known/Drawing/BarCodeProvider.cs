using System.Drawing;

namespace Known.Drawing
{
    public interface IBarCodeProvider
    {
        Bitmap CreateBarCode(string content);
        string GetBarCodeContent(Bitmap bitmap);
        Bitmap CreateQrCode(string content);
        string GetQrCodeContent(Bitmap bitmap);
    }
}
