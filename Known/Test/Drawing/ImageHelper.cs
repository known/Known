using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Known.Drawing
{
    /// <summary>
    /// 图片帮助者类。
    /// </summary>
    public sealed class ImageHelper
    {
        /// <summary>
        /// 创建指定内容的条形码。
        /// </summary>
        /// <param name="provider">条形码提供者对象。</param>
        /// <param name="content">条形码内容。</param>
        /// <returns>条形码图片。</returns>
        public static Bitmap CreateBarCode(IBarCodeProvider provider, string content)
        {
            return provider.CreateBarCode(content);
        }

        /// <summary>
        /// 获取条形码图片的内容。
        /// </summary>
        /// <param name="provider">条形码提供者对象。</param>
        /// <param name="bitmap">条形码图片。</param>
        /// <returns>条形码内容。</returns>
        public static string GetBarCodeContent(IBarCodeProvider provider, Bitmap bitmap)
        {
            return provider.GetBarCodeContent(bitmap);
        }

        /// <summary>
        /// 创建指定内容的二维码。
        /// </summary>
        /// <param name="provider">二维码提供者对象。</param>
        /// <param name="content">二维码内容。</param>
        /// <param name="logoFile">二维码中间 LOGO 文件路径，默认空。</param>
        /// <returns>二维码图片。</returns>
        public static Bitmap CreateQrCode(IBarCodeProvider provider, string content, string logoFile = null)
        {
            var image = provider.CreateQrCode(content);
            if (!string.IsNullOrWhiteSpace(logoFile))
            {
                using (var btm = new Bitmap(logoFile))
                using (var logo = new Bitmap(btm, image.Width / 5, image.Height / 5))
                using (var g = Graphics.FromImage(image))
                {
                    var x = image.Width / 2 - logo.Width / 2;
                    var y = image.Height / 2 - logo.Height / 2;
                    g.DrawImage(logo, x, y);
                }
            }

            return image;
        }

        /// <summary>
        /// 获取二维码图片的内容。
        /// </summary>
        /// <param name="provider">二维码提供者对象。</param>
        /// <param name="bitmap">二维码图片。</param>
        /// <returns>二维码内容。</returns>
        public static string GetQrCodeContent(IBarCodeProvider provider, Bitmap bitmap)
        {
            return provider.GetQrCodeContent(bitmap);
        }

        /// <summary>
        /// 生成随机验证码图片。
        /// </summary>
        /// <param name="length">验证码长度。</param>
        /// <param name="code">验证码内容。</param>
        /// <param name="provider">验证码提供者对象，默认空。</param>
        /// <returns>验证码图片字节。</returns>
        public static byte[] CreateCaptcha(int length, out string code, ICaptchaProvider provider = null)
        {
            if (provider == null)
                provider = new DefaultCaptchaProvider();

            code = provider.GetRandomCode(length);
            return provider.GenerateImage(code);
        }

        /// <summary>
        /// 旋转图片。
        /// </summary>
        /// <param name="fileName">图片文件路径。</param>
        /// <param name="angle">旋转角度，正数表示顺时针，负数表示逆时针。</param>
        /// <returns>旋转后的图片。</returns>
        public static Image RotateImage(string fileName, int angle)
        {
            var b = Image.FromFile(fileName);
            angle %= 360;

            var radian = angle * Math.PI / 180.0;
            var cos = Math.Cos(radian);
            var sin = Math.Sin(radian);

            var w = b.Width;
            var h = b.Height;
            var W = (int)(Math.Max(Math.Abs(w * cos - h * sin), Math.Abs(w * cos + h * sin)));
            var H = (int)(Math.Max(Math.Abs(w * sin - h * cos), Math.Abs(w * sin + h * cos)));

            var dsImage = new Bitmap(W, H);
            var g = Graphics.FromImage(dsImage);
            g.InterpolationMode = InterpolationMode.Bilinear;
            g.SmoothingMode = SmoothingMode.HighQuality;

            var offset = new Point((W - w) / 2, (H - h) / 2);
            var rect = new Rectangle(offset.X, offset.Y, w, h);
            var center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(360 - angle);
            g.TranslateTransform(-center.X, -center.Y);
            g.DrawImage(b, rect);
            g.ResetTransform();
            g.Save();
            g.Dispose();

            b.Dispose();
            dsImage.Save(fileName, ImageFormat.Jpeg);
            return dsImage;
        }

        /// <summary>
        /// 调整图片的大小。
        /// </summary>
        /// <param name="source">源图片对象。</param>
        /// <param name="width">调整后的宽度。</param>
        /// <param name="height">调整后的高度。</param>
        /// <returns>调整后的图片。</returns>
        public static Image ResizeImage(Image source, int width, int height)
        {
            if (source == null)
                return null;

            var target = new Bitmap(width, height);
            using(var graphic = Graphics.FromImage(target))
            {
                var destRect = new Rectangle(0, 0, width, height);
                var srcRect = new Rectangle(0, 0, source.Width, source.Height);
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.DrawImage(source, destRect, srcRect, GraphicsUnit.Pixel);
            }
            return target;
        }

        /// <summary>
        /// 保存剪切图。
        /// </summary>
        /// <param name="stream">源图片文件流。</param>
        /// <param name="fileName">保存剪切后的文件路径。</param>
        /// <param name="option">剪切图片的选择。</param>
        public static void SaveCutImage(Stream stream, string fileName, ImageOption option)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

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
        /// 保存图片的缩略图。
        /// </summary>
        /// <param name="stream">源图片文件流。</param>
        /// <param name="fileName">缩略图文件路径。</param>
        /// <param name="option">缩略图选项。</param>
        public static void SaveThumbnail(Stream stream, string fileName, ImageOption option)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            var image = Image.FromStream(stream, true);
            var adjustSize = Helper.AdjustSize(option.Width, option.Height, image.Width, image.Height);
            var thumbnail = image.GetThumbnailImage(adjustSize.Width, adjustSize.Height, new Image.GetThumbnailImageAbort(Helper.ThumbnailCallback), IntPtr.Zero);
            if (!string.IsNullOrWhiteSpace(option.WatermarkText))
            {
                using (var gWater = Graphics.FromImage(thumbnail))
                {
                    var fontWater = new Font(FontFamily.GenericSansSerif, 10);
                    var brushWater = new SolidBrush(Color.BlueViolet);
                    gWater.DrawString(option.WatermarkText, fontWater, brushWater, 10, 10);
                }
            }

            using (var bitmap = new Bitmap(thumbnail))
            {
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

        /// <summary>
        /// 保存图片的缩略图。
        /// </summary>
        /// <param name="sourceFile">源图片文件路径。</param>
        /// <param name="destFile">目标图片文件路径。</param>
        /// <param name="destWidth">目标图片宽度。</param>
        /// <param name="destHeight">目标图片高度。</param>
        /// <param name="rate">图标图片质量，1-100之间的数。</param>
        /// <returns>是否保存成功。</returns>
        public static bool SaveThumbnail(string sourceFile, string destFile, int destWidth, int destHeight, int rate)
        {
            var iSource = Image.FromFile(sourceFile);
            var tFormat = iSource.RawFormat;
            var tem_size = new Size(iSource.Width, iSource.Height);
            int sW;
            int sH;
            if (tem_size.Width > destHeight || tem_size.Width > destWidth)
            {
                if ((tem_size.Width * destHeight) > (tem_size.Height * destWidth))
                {
                    sW = destWidth;
                    sH = (destWidth * tem_size.Height) / tem_size.Width;
                }
                else
                {
                    sH = destHeight;
                    sW = (tem_size.Width * destHeight) / tem_size.Height;
                }
            }
            else
            {
                sW = tem_size.Width;
                sH = tem_size.Height;
            }

            var ob = new Bitmap(destWidth, destHeight);
            var g = Graphics.FromImage(ob);
            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(iSource, new Rectangle((destWidth - sW) / 2, (destHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);
            g.Dispose();

            var ep = new EncoderParameters();
            var qy = new long[1];
            qy[0] = rate;//rate:1-100
            var eParam = new EncoderParameter(Encoder.Quality, qy);
            ep.Param[0] = eParam;

            try
            {
                var arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;

                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }

                if (jpegICIinfo != null)
                {
                    ob.Save(destFile, jpegICIinfo, ep);
                }
                else
                {
                    ob.Save(destFile, tFormat);
                }

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
            }
        }

        /// <summary>
        /// 将 Tiff 格式文件转成 Jpg 格式。
        /// </summary>
        /// <param name="tifFile">tiff 文件路径。</param>
        /// <param name="jpgFile">jpg 文件路径。</param>
        public static void TiffToJpg(string tifFile, string jpgFile)
        {
            var files = new List<string>();
            var file = new FileInfo(tifFile);
            using (var stream = file.OpenRead())
            {
                var path = Path.GetDirectoryName(jpgFile);
                var createPath = Path.Combine(path, "Combine");
                if (!Directory.Exists(createPath))
                    Directory.CreateDirectory(createPath);

                var jpgFileName = Path.GetFileNameWithoutExtension(jpgFile);
                var bmp = new Bitmap(stream);
                Image image = bmp;
                var guid = image.FrameDimensionsList[0];
                var dimension = new FrameDimension(guid);
                var frameCount = image.GetFrameCount(dimension);
                for (int i = 0; i < frameCount; i++)
                {
                    var tmpJpg = createPath + jpgFileName + i + ".jpg";
                    image.SelectActiveFrame(dimension, i);
                    image.Save(tmpJpg, ImageFormat.Jpeg);
                    files.Add(tmpJpg);
                }
                image.Dispose();
                stream.Close();
                CombineImages(files, jpgFile);
            }
            files.ForEach(f => File.Delete(f));
        }

        /// <summary>
        /// 合并多个图片。
        /// </summary>
        /// <param name="sourceFiles">源图片文件列表。</param>
        /// <param name="destFile">目标图片文件路径。</param>
        /// <param name="mergeType">合并方向，默认纵向。</param>
        public static void CombineImages(List<string> sourceFiles, string destFile, ImageMergeOrientation mergeType = ImageMergeOrientation.Vertical)
        {
            var finalImage = destFile;
            var finalImageBak = destFile.Insert(destFile.LastIndexOf('.'), "_BAK");
            var images = sourceFiles.Select(f =>
            {
                var tmp = Image.FromFile(f);
                var img = new Bitmap(tmp);
                tmp.Dispose();
                return img;
            }).ToList();
            var finalWidth = mergeType == ImageMergeOrientation.Horizontal
                           ? images.Sum(img => img.Width)
                           : images.Max(img => img.Width);
            var finalHeight = mergeType == ImageMergeOrientation.Vertical
                            ? images.Sum(img => img.Height)
                            : images.Max(img => img.Height);

            var finalImg = new Bitmap(finalWidth, finalHeight);
            var g = Graphics.FromImage(finalImg);
            g.Clear(SystemColors.AppWorkspace);

            var x = 0;
            var y = 0;
            foreach (var img in images)
            {
                g.DrawImage(img, x, y, img.Width, img.Height);
                switch (mergeType)
                {
                    case ImageMergeOrientation.Horizontal:
                        x += img.Width;
                        break;
                    case ImageMergeOrientation.Vertical:
                        y += img.Height;
                        break;
                    default:
                        break;
                }
                img.Dispose();
            }
            g.Dispose();
            finalImg.Save(finalImageBak, ImageFormat.Tiff);
            finalImg.Dispose();
            SaveThumbnail(finalImageBak, finalImage, finalWidth, finalHeight, 40);
            File.Delete(finalImageBak);
        }

        class Helper
        {
            internal static Size AdjustSize(int spcWidth, int spcHeight, int orgWidth, int orgHeight)
            {
                var size = new Size();
                if (orgWidth <= spcWidth && orgHeight <= spcHeight)
                {
                    size.Width = orgWidth;
                    size.Height = orgHeight;
                }
                else
                {
                    float w = orgWidth / (float)spcWidth;
                    float h = orgHeight / (float)spcHeight;
                    if (w > h)
                    {
                        size.Width = spcWidth;
                        size.Height = (int)(w >= 1 ? Math.Round(orgHeight / w) : Math.Round(orgHeight * w));
                    }
                    else if (w < h)
                    {
                        size.Height = spcHeight;
                        size.Width = (int)(h >= 1 ? Math.Round(orgWidth / h) : Math.Round(orgWidth * h));
                    }
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
