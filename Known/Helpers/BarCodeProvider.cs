using System.Drawing;

namespace Known.Helpers
{
    public interface IBarCodeProvider
    {
        /// <summary>
        /// 根据内容创建条形码图片。
        /// </summary>
        /// <param name="content">条形码内容。</param>
        /// <returns>条形码图片。</returns>
        Bitmap CreateBarCode(string content);

        /// <summary>
        /// 获取条形码内容。
        /// </summary>
        /// <param name="bitmap">条形码图片。</param>
        /// <returns>条形码内容。</returns>
        string GetBarCodeContent(Bitmap bitmap);

        /// <summary>
        /// 根据内容创建二维码。
        /// </summary>
        /// <param name="content">二维码内容。</param>
        /// <returns>二维码图片。</returns>
        Bitmap CreateQrCode(string content);

        /// <summary>
        /// 获取二维码内容。
        /// </summary>
        /// <param name="bitmap">二维码图片。</param>
        /// <returns>二维码内容。</returns>
        string GetQrCodeContent(Bitmap bitmap);
    }
}
