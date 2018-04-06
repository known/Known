using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Known.Helpers
{
    /// <summary>
    /// 图片帮助者。
    /// </summary>
    public sealed class ImageHelper
    {
        /// <summary>
        /// 创建验证码图片。
        /// </summary>
        /// <param name="length">验证码字符长度。</param>
        /// <param name="code">验证码字符串。</param>
        /// <returns>验证码图片字节。</returns>
        public static byte[] CreateCaptcha(int length, out string code)
        {
            var randMembers = new int[length];
            var validateNums = new int[length];
            var validateNumberStr = string.Empty;
            //生成起始序列值
            var seekSeek = unchecked((int)DateTime.Now.Ticks);
            var seekRand = new Random(seekSeek);
            var beginSeek = seekRand.Next(0, int.MaxValue - length * 10000);
            var seeks = new int[length];
            for (var i = 0; i < length; i++)
            {
                beginSeek += 10000;
                seeks[i] = beginSeek;
            }
            //生成随机数字
            for (var i = 0; i < length; i++)
            {
                var rand = new Random(seeks[i]);
                var pownum = 1 * (int)Math.Pow(10, length);
                randMembers[i] = rand.Next(pownum, int.MaxValue);
            }
            //抽取随机数字
            for (var i = 0; i < length; i++)
            {
                var numStr = randMembers[i].ToString();
                var numLength = numStr.Length;
                var rand = new Random();
                var numPosition = rand.Next(0, numLength - 1);
                validateNums[i] = int.Parse(numStr.Substring(numPosition, 1));
            }
            //生成验证码
            for (var i = 0; i < length; i++)
            {
                validateNumberStr += validateNums[i].ToString();
            }

            code = validateNumberStr;
            var image = new Bitmap((int)Math.Ceiling(code.Length * 12.0), 22);
            var g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器
                var random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的干扰线
                for (var i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }
                var font = new Font("Arial", 12, (FontStyle.Bold | FontStyle.Italic));
                var brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(code, font, brush, 3, 2);
                //画图片的前景干扰点
                for (var i = 0; i < 100; i++)
                {
                    var x = random.Next(image.Width);
                    var y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                //保存图片数据
                using (var stream = new MemoryStream())
                {
                    image.Save(stream, ImageFormat.Jpeg);
                    //输出图片流
                    return stream.ToArray();
                }
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }

        /// <summary>
        /// 旋转图片。
        /// </summary>
        /// <param name="fileName">要旋转的图片文件路径。</param>
        /// <param name="angle">要旋转的角度。</param>
        /// <returns>旋转后图片。</returns>
        public static Image RotateImage(string fileName, int angle)
        {
            var b = Image.FromFile(fileName);
            angle = angle % 360;

            //弧度转换  
            var radian = angle * Math.PI / 180.0;
            var cos = Math.Cos(radian);
            var sin = Math.Sin(radian);

            //原图的宽和高  
            var w = b.Width;
            var h = b.Height;
            var W = (int)(Math.Max(Math.Abs(w * cos - h * sin), Math.Abs(w * cos + h * sin)));
            var H = (int)(Math.Max(Math.Abs(w * sin - h * cos), Math.Abs(w * sin + h * cos)));

            //目标位图  
            var dsImage = new Bitmap(W, H);
            var g = Graphics.FromImage(dsImage);
            g.InterpolationMode = InterpolationMode.Bilinear;
            g.SmoothingMode = SmoothingMode.HighQuality;

            //计算偏移量  
            var offset = new Point((W - w) / 2, (H - h) / 2);

            //构造图像显示区域：让图像的中心与窗口的中心点一致  
            var rect = new Rectangle(offset.X, offset.Y, w, h);
            var center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(360 - angle);

            //恢复图像在水平和垂直方向的平移  
            g.TranslateTransform(-center.X, -center.Y);
            g.DrawImage(b, rect);

            //重至绘图的所有变换  
            g.ResetTransform();
            g.Save();
            g.Dispose();

            //保存旋转后的图片  
            b.Dispose();
            dsImage.Save(fileName, ImageFormat.Jpeg);
            return dsImage;
        }

        /// <summary>
        /// 保存切割的图片。
        /// </summary>
        /// <param name="stream">要切割的图片文件流。</param>
        /// <param name="fileName">保存切割后的文件路径。</param>
        /// <param name="option">切割图片参数选项。</param>
        public static void SaveCutImage(Stream stream, string fileName, ImageOption option)
        {
            Utils.EnsureFile(fileName);
            var image = Image.FromStream(stream);
            var x = option.X;
            var y = option.Y;
            var srcRect = new Rectangle(x, y, option.Width, option.Height);
            var destRect = new Rectangle(0, 0, option.Width, option.Height);
            using (var bitmap = new Bitmap(option.Width, option.Height, PixelFormat.Format32bppArgb))
            using (var graphic = Graphics.FromImage(bitmap))
            {
                graphic.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
                bitmap.Save(fileName, ImageFormat.Png);
            }
        }

        /// <summary>
        /// 保存图片为缩略图。
        /// </summary>
        /// <param name="stream">要缩略的图片文件流。</param>
        /// <param name="fileName">保存缩略图的文件路径。</param>
        /// <param name="option">图片参数选项。</param>
        public static void SaveThumbnail(Stream stream, string fileName, ImageOption option)
        {
            Utils.EnsureFile(fileName);
            var image = Image.FromStream(stream, true);
            var adjustSize = Helper.AdjustSize(option.Width, option.Height, image.Width, image.Height);
            var thumbnail = image.GetThumbnailImage(adjustSize.Width, adjustSize.Height, new Image.GetThumbnailImageAbort(Helper.ThumbnailCallback), IntPtr.Zero);
            if (!string.IsNullOrWhiteSpace(option.WatermarkText))
            {
                using (var gWater = Graphics.FromImage(thumbnail))
                {
                    var fontWater = new Font("黑体", 10);
                    var brushWater = new SolidBrush(Color.White);
                    gWater.DrawString(option.WatermarkText, fontWater, brushWater, 10, 10);
                }
            }

            using (var bitmap = new Bitmap(thumbnail))
            {
                //处理JPG质量的函数  
                var ici = Helper.GetEncoderInfo("image/jpeg");
                if (ici != null)
                {
                    using (var ep = new EncoderParameters(1))
                    {
                        ep.Param[0] = new EncoderParameter(Encoder.Quality, (long)100);
                        bitmap.Save(fileName, ici, ep);
                    }
                }
                ici = null;
            }

            thumbnail.Dispose();
            thumbnail = null;
            image.Dispose();
            image = null;
        }

        class Helper
        {
            internal static Size AdjustSize(int spcWidth, int spcHeight, int orgWidth, int orgHeight)
            {
                var size = new Size();
                // 原始宽高在指定宽高范围内，不作任何处理   
                if (orgWidth <= spcWidth && orgHeight <= spcHeight)
                {
                    size.Width = orgWidth;
                    size.Height = orgHeight;
                }
                else
                {
                    // 取得比例系数   
                    float w = orgWidth / (float)spcWidth;
                    float h = orgHeight / (float)spcHeight;
                    // 宽度比大于高度比   
                    if (w > h)
                    {
                        size.Width = spcWidth;
                        size.Height = (int)(w >= 1 ? Math.Round(orgHeight / w) : Math.Round(orgHeight * w));
                    }
                    // 宽度比小于高度比   
                    else if (w < h)
                    {
                        size.Height = spcHeight;
                        size.Width = (int)(h >= 1 ? Math.Round(orgWidth / h) : Math.Round(orgWidth * h));
                    }
                    // 宽度比等于高度比   
                    else
                    {
                        size.Width = spcWidth;
                        size.Height = spcHeight;
                    }
                }
                return size;
            }

            internal static ImageCodecInfo GetEncoderInfo(string mimeType)
            {
                int j;
                var encoders = ImageCodecInfo.GetImageEncoders();
                for (j = 0; j < encoders.Length; ++j)
                {
                    if (encoders[j].MimeType == mimeType)
                        return encoders[j];
                }
                return null;
            }

            internal static bool ThumbnailCallback()
            {
                return false;
            }
        }
    }
}
