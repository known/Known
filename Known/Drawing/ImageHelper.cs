using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Known.Drawing
{
    public sealed class ImageHelper
    {
        public static Bitmap CreateBarCode(IBarCodeProvider provider, string content)
        {
            return provider.CreateBarCode(content);
        }

        public static string GetBarCodeContent(IBarCodeProvider provider, Bitmap bitmap)
        {
            return provider.GetBarCodeContent(bitmap);
        }

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

        public static string GetQrCodeContent(IBarCodeProvider provider, Bitmap bitmap)
        {
            return provider.GetQrCodeContent(bitmap);
        }

        public static byte[] CreateCaptcha(int length, out string code, ICaptchaProvider provider = null)
        {
            if (provider == null)
                provider = new DefaultCaptchaProvider();

            code = provider.GetRandomCode(length);
            return provider.GenerateImage(code);
        }

        public static Image RotateImage(string fileName, int angle)
        {
            var b = Image.FromFile(fileName);
            angle = angle % 360;

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

        public static bool SaveThumbnail(string sourceFile, string destFile, int destWidth, int destHeight, int rate)
        {
            var iSource = Image.FromFile(sourceFile);
            var tFormat = iSource.RawFormat;
            int sW = 0, sH = 0;

            var tem_size = new Size(iSource.Width, iSource.Height);

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
