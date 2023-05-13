using System.Net.NetworkInformation;

namespace Known.Core;

class WebPlatform : IPlatform
{
    public string GetMacAddress()
    {
        var nics = NetworkInterface.GetAllNetworkInterfaces();
        if (nics == null || nics.Length < 1)
            return string.Empty;

        foreach (var adapter in nics)
        {
            var ips = adapter.GetIPProperties();
            var addresses = ips.UnicastAddresses;
            foreach (var item in addresses)
            {
                if (item.IsDnsEligible)
                {
                    var address = adapter.GetPhysicalAddress();
                    var hexs = address.GetAddressBytes().Select(b => b.ToString("X2")).ToArray();
                    return string.Join(":", hexs);
                }
            }
        }

        return string.Empty;
    }

    public string GetIPAddress()
    {
        var nics = NetworkInterface.GetAllNetworkInterfaces();
        if (nics == null || nics.Length < 1)
            return string.Empty;

        var ip = string.Empty;
        foreach (var adapter in nics)
        {
            var ips = adapter.GetIPProperties();
            var addresses = ips.UnicastAddresses;
            foreach (var item in addresses)
            {
                if (item.IsDnsEligible)
                {
                    ip = item.Address.ToString();
                }
            }
        }

        return ip;
    }

    public void MakeThumbnail(Stream stream, string thumbnailPath, int width, int height)
    {
        var source = Image.FromStream(stream);
        GetPicThumbnail(source, thumbnailPath, width, height, 80);
        stream.Close();
    }

    private static bool GetPicThumbnail(Image iSource, string destFile, int destHeight, int destWidth, int flag)
    {
        var tFormat = iSource.RawFormat;
        int sW = 0, sH = 0;

        //按比例缩放
        var tem_size = new Size(iSource.Width, iSource.Height);
        if (tem_size.Width > destHeight || tem_size.Width > destWidth)
        {
            if (tem_size.Width * destHeight > tem_size.Height * destWidth)
            {
                sW = destWidth;
                sH = destWidth * tem_size.Height / tem_size.Width;
            }
            else
            {
                sH = destHeight;
                sW = tem_size.Width * destHeight / tem_size.Height;
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
        qy[0] = flag;//设置压缩的比例1-100
        var eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
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
                ob.Save(destFile, jpegICIinfo, ep);//dFile是压缩后的新路径
            else
                ob.Save(destFile, tFormat);

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
}