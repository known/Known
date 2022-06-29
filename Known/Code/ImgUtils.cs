/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-06-29     KnownChen    初始化
 * ------------------------------------------------------------------------------- */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Known
{
    public class ImgUtils
    {
        public static byte[] CreateCaptcha(int length, out string code)
        {
            code = string.Empty;
            var textArray = "0,1,2,3,4,5,6,7,8,9,A,C,D,E,F,G,H,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y".Split(',');
            for (var i = 0; i < length; i++)
            {
                var seekRand = new Random(i * (int)DateTime.Now.Ticks);
                var index = seekRand.Next(textArray.Length);
                code += textArray[index];
            }

            MemoryStream ms = null;
            Color[] colors = { Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
            string[] fonts = { "Arial", "Verdana", "Microsoft Sans Serif", "Comic Sans MS" };

            using (var img = new Bitmap(code.Length * 18, 32))
            {
                using (var g = Graphics.FromImage(img))
                {
                    g.Clear(Color.White);

                    var random = new Random();
                    //在随机位置画背景点  
                    for (int i = 0; i < 250; i++)
                    {
                        int x = random.Next(img.Width);
                        int y = random.Next(img.Height);
                        g.DrawRectangle(new Pen(Color.LightGray, 0), x, y, 1, 1);
                    }
                    //验证码绘制在g中  
                    for (int i = 0; i < code.Length; i++)
                    {
                        int cindex = random.Next(colors.Length);//随机颜色索引值  
                        int findex = random.Next(fonts.Length);//随机字体索引值  
                        Font f = new Font(fonts[findex], 15, FontStyle.Bold);//字体  
                        Brush b = new SolidBrush(colors[cindex]);//颜色  
                        int ii = 4;
                        if ((i + 1) % 2 == 0)//控制验证码不在同一高度  
                        {
                            ii = 2;
                        }
                        g.DrawString(code.Substring(i, 1), f, b, 3 + (i * 16), ii);//绘制一个验证字符  
                    }
                    ms = new MemoryStream();//生成内存流对象  
                    img.Save(ms, ImageFormat.Jpeg);//将此图像以Png图像文件的格式保存到流中
                }
            }

            return ms.ToArray();
        }
    }
}