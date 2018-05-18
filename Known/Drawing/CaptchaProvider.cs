using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Known.Drawing
{
    /// <summary>
    /// 验证码提供者接口。
    /// </summary>
    public interface ICaptchaProvider
    {
        /// <summary>
        /// 获取随机验证码字符串。
        /// </summary>
        /// <param name="length">验证码长度。</param>
        /// <returns>随机验证码字符串。</returns>
        string GetRandomCode(int length);

        /// <summary>
        /// 生成验证码图片，返回字节流。
        /// </summary>
        /// <param name="code">验证码。</param>
        /// <returns>验证码图片字节流。</returns>
        byte[] GenerateImage(string code);
    }

    /// <summary>
    /// 默认验证码提供者。
    /// </summary>
    public class DefaultCaptchaProvider : ICaptchaProvider
    {
        /// <summary>
        /// 获取随机验证码字符串。
        /// </summary>
        /// <param name="length">验证码长度。</param>
        /// <returns>随机验证码字符串。</returns>
        public string GetRandomCode(int length)
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

            return validateNumberStr;
        }

        /// <summary>
        /// 生成验证码图片，返回字节流。
        /// </summary>
        /// <param name="code">验证码。</param>
        /// <returns>验证码图片字节流。</returns>
        public byte[] GenerateImage(string code)
        {
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
    }
}
