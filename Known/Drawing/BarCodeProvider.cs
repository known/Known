using System.Drawing;

namespace Known.Drawing
{
    /// <summary>
    /// 条形码和二维码提供者接口。
    /// </summary>
    public interface IBarCodeProvider
    {
        /// <summary>
        /// 创建指定内容的条形码。
        /// </summary>
        /// <param name="content">条形码内容。</param>
        /// <returns>条形码位图。</returns>
        Bitmap CreateBarCode(string content);

        /// <summary>
        /// 获取条形码图片的内容。
        /// </summary>
        /// <param name="bitmap">条形码图片。</param>
        /// <returns>条形码内容。</returns>
        string GetBarCodeContent(Bitmap bitmap);

        /// <summary>
        /// 创建指定内容的二维码。
        /// </summary>
        /// <param name="content">二维码内容。</param>
        /// <returns>二维码图片。</returns>
        Bitmap CreateQrCode(string content);

        /// <summary>
        /// 获取二维码图片的内容。
        /// </summary>
        /// <param name="bitmap">二维码图片。</param>
        /// <returns>二维码内容。</returns>
        string GetQrCodeContent(Bitmap bitmap);
    }
}
