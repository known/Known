using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Known.Drawing
{
    public interface ICaptchaProvider
    {
        string GetRandomCode(int length);
        byte[] GenerateImage(string code);
    }

    class DefaultCaptchaProvider : ICaptchaProvider
    {
        public string GetRandomCode(int length)
        {
            var randMembers = new int[length];
            var validateNums = new int[length];
            var validateNumberStr = string.Empty;
            //generate a start value
            var seekSeek = unchecked((int)DateTime.Now.Ticks);
            var seekRand = new Random(seekSeek);
            var beginSeek = seekRand.Next(0, int.MaxValue - length * 10000);
            var seeks = new int[length];
            for (var i = 0; i < length; i++)
            {
                beginSeek += 10000;
                seeks[i] = beginSeek;
            }
            //generate radom number
            for (var i = 0; i < length; i++)
            {
                var rand = new Random(seeks[i]);
                var pownum = 1 * (int)Math.Pow(10, length);
                randMembers[i] = rand.Next(pownum, int.MaxValue);
            }
            //fetch radom number
            for (var i = 0; i < length; i++)
            {
                var numStr = randMembers[i].ToString();
                var numLength = numStr.Length;
                var rand = new Random();
                var numPosition = rand.Next(0, numLength - 1);
                validateNums[i] = int.Parse(numStr.Substring(numPosition, 1));
            }
            //generate captcha
            for (var i = 0; i < length; i++)
            {
                validateNumberStr += validateNums[i].ToString();
            }

            return validateNumberStr;
        }

        public byte[] GenerateImage(string code)
        {
            var image = new Bitmap((int)Math.Ceiling(code.Length * 12.0), 22);
            var g = Graphics.FromImage(image);
            try
            {
                var random = new Random();
                g.Clear(Color.White);
                //drawing interference line
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
                //drawing front interference point
                for (var i = 0; i < 100; i++)
                {
                    var x = random.Next(image.Width);
                    var y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //drawing border
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                using (var stream = new MemoryStream())
                {
                    image.Save(stream, ImageFormat.Jpeg);
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
