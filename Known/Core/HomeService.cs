/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * 2022-06-23     KnownChen    优化用户管理及登录
 * ------------------------------------------------------------------------------- */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Known.Web;

namespace Known.Core
{
    class HomeService : ServiceBase, IService
    {
        #region Login
        [Anonymous, Route("signin")]
        public Result SignIn(string userName, string password, string captcha, string cid, bool force)
        {
            if (string.IsNullOrEmpty(captcha) || captcha.ToUpper() != CaptchaCode)
                return Result.Error(Language.NotCaptcha);

            userName = userName.ToLower();
            var result = Platform.SignIn(userName, password, cid, force);
            if (!result.IsValid)
                return result;

            return Result.Success(Language.LoginOK, new { User = result.Data });
        }

        [Anonymous, Route("signToken")]
        public Result SignInByToken(string token)
        {
            return Platform.SignInByToken(token);
        }

        [Route("signout")]
        public Result SignOut()
        {
            Platform.SignOut(CurrentUser.UserName);
            return Result.Success(Language.LogoutOK);
        }

        public object GetUserData(string appId)
        {
            var user = CurrentUser;
            if (user == null)
                return null;

            if (string.IsNullOrEmpty(appId))
                appId = user.AppId;

            var codes = Platform.GetCodes(appId, user.CompNo);
            var data = Platform.GetUserMenus(appId, user.UserName, true);
            var menus = data.Select(m => m.ToTree());
            return new { user, menus, codes };
        }
        #endregion

        #region Help
        public string GetHelp(string hid) => string.Empty;
        #endregion

        #region Captcha
        private string CaptchaCode
        {
            get
            {
                var code = Context.GetSession<string>("CaptchaCode");
                if (code == null)
                    return string.Empty;

                return code.ToString().ToUpper();
            }
            set { Context.SetSession("CaptchaCode", value); }
        }

        [Anonymous]
        public ApiFile GetCaptcha()
        {
            var bytes = CreateCaptcha(4, out string code);
            CaptchaCode = code;
            return new ApiFile
            {
                ContentType = MimeTypes.ImageJpeg,
                Bytes = bytes
            };
        }

        private static byte[] CreateCaptcha(int length, out string code)
        {
            code = string.Empty;
            var textArray = "2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y,Z".Split(',');
            for (var i = 0; i < length; i++)
            {
                var seekRand = new Random(i * (int)DateTime.Now.Ticks);
                var index = seekRand.Next(textArray.Length);
                code += textArray[index];
            }

            MemoryStream ms = null;
            Color[] colors = { Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
            string[] fonts = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial" };

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
        #endregion

        #region Active
        [Anonymous]
        public object Install(string data)
        {
            if (data == "1")
                return Platform.GetInstall();

            var info = Utils.FromJson<InstallInfo>(data);
            var result = Platform.SaveSystem(info);
            if (result.IsValid)
            {
                Config.Init();
            }
            return result;
        }
        #endregion

        #region Profile
        public UserInfo GetUserInfo()
        {
            return Platform.GetUserInfo(CurrentUser.UserName);
        }

        public string GetUserHistory()
        {
            var user = CurrentUser;
            return History.GetHistory(user);
        }

        public Result SaveUserInfo(string data)
        {
            return PostAction<UserInfo>(data, d => Platform.SaveUserInfo(d));
        }

        public Result UpdatePassword(string data)
        {
            return PostAction<PasswordInfo>(data, d => Platform.UpdatePassword(d.OldPassword, d.NewPassword, d.NewPassword1));
        }
        #endregion
    }

    class PasswordInfo
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPassword1 { get; set; }
    }
}