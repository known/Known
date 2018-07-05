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
    /// 图片帮助者。
    /// </summary>
    public sealed class ImageHelper
    {
        /// <summary>
        /// 根据内容创建条形码图片。
        /// </summary>
        /// <param name="provider">条码提供者。</param>
        /// <param name="content">条形码内容。</param>
        /// <returns>条形码图片。</returns>
        public static Bitmap CreateBarCode(IBarCodeProvider provider, string content)
        {
            return provider.CreateBarCode(content);
        }

        /// <summary>
        /// 获取条形码内容。
        /// </summary>
        /// <param name="provider">条码提供者。</param>
        /// <param name="bitmap">条形码图片。</param>
        /// <returns>条形码内容。</returns>
        public static string GetBarCodeContent(IBarCodeProvider provider, Bitmap bitmap)
        {
            return provider.GetBarCodeContent(bitmap);
        }

        /// <summary>
        /// 根据内容创建二维码。
        /// </summary>
        /// <param name="provider">条码提供者。</param>
        /// <param name="content">二维码内容。</param>
        /// <param name="logoFile">二维码图标文件。</param>
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
        /// 获取二维码内容。
        /// </summary>
        /// <param name="provider">条码提供者。</param>
        /// <param name="bitmap">二维码图片。</param>
        /// <returns>二维码内容。</returns>
        public static string GetQrCodeContent(IBarCodeProvider provider, Bitmap bitmap)
        {
            return provider.GetQrCodeContent(bitmap);
        }

        /// <summary>
        /// 创建验证码图片。
        /// </summary>
        /// <param name="length">验证码字符长度。</param>
        /// <param name="code">验证码字符串。</param>
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
        /// 调整图片大小。
        /// </summary>
        /// <param name="source">调整前的图片。</param>
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
        /// 保存切割的图片。
        /// </summary>
        /// <param name="stream">要切割的图片文件流。</param>
        /// <param name="fileName">保存切割后的文件路径。</param>
        /// <param name="option">切割图片参数选项。</param>
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
        /// 保存图片为缩略图。
        /// </summary>
        /// <param name="stream">要缩略的图片文件流。</param>
        /// <param name="fileName">保存缩略图的文件路径。</param>
        /// <param name="option">图片参数选项。</param>
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
                    var fontWater = new Font("黑体", 10);
                    var brushWater = new SolidBrush(Color.BlueViolet);
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

        /// <summary>
        /// 保存图片为缩略图。
        /// </summary>
        /// <param name="sourceFile">源图片文件路径。</param>
        /// <param name="destFile">目标图片文件路径。</param>
        /// <param name="destWidth">目标图片宽度。</param>
        /// <param name="destHeight">目标图片高度。</param>
        /// <param name="rate">压缩的比例1-100。</param>
        /// <returns>是否保存成功。</returns>
        public static bool SaveThumbnail(string sourceFile, string destFile, int destWidth, int destHeight, int rate)
        {
            var iSource = Image.FromFile(sourceFile);
            var tFormat = iSource.RawFormat;
            int sW = 0, sH = 0;

            //按比例缩放
            var tem_size = new Size(iSource.Width, iSource.Height);

            if (tem_size.Width > destHeight || tem_size.Width > destWidth) //将**改成c#中的或者操作符号
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

            //以下代码为保存图片时，设置压缩质量
            var ep = new EncoderParameters();
            var qy = new long[1];
            qy[0] = rate;//设置压缩的比例1-100
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
                    ob.Save(destFile, jpegICIinfo, ep);//dFile是压缩后的新路径
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
        /// TIFF文件转成JPG图片。
        /// </summary>
        /// <param name="tifFile">TIF文件路径。</param>
        /// <param name="jpgFile">JPG文件路径。</param>
        public static void TiffToJpg(string tifFile, string jpgFile)
        {
            var files = new List<string>();
            var file = new FileInfo(tifFile);
            using (var stream = file.OpenRead())
            {
                var bmp = new Bitmap(stream);
                Image image = bmp;
                var guid = image.FrameDimensionsList[0];
                var dimension = new FrameDimension(guid);
                int frameCount = image.GetFrameCount(dimension);
                var path = Path.GetDirectoryName(jpgFile);
                var createPath = Path.Combine(path, "Combine");
                if (!Directory.Exists(createPath))
                    Directory.CreateDirectory(createPath);
                var jpgFileName = Path.GetFileNameWithoutExtension(jpgFile);
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
        /// 合并多张图片。
        /// </summary>
        /// <param name="sourceFiles">源图片文件集合。</param>
        /// <param name="destFile">目标图片文件路径。</param>
        /// <param name="mergeType">图片合并方向。</param>
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
