using Known.Web;

namespace Known
{
    /// <summary>
    /// 应用程序全局设定类。
    /// </summary>
    public class Setting
    {
        private Setting()
        {
            App = new AppInfo
            {
                Id = Config.AppSetting("AppId"),
                Name = Config.AppSetting("AppName")
            };
            ApiPlatformUrl = Config.AppSetting("ApiPlatformUrl");
            ApiBaseUrl = Config.AppSetting("ApiBaseUrl");
            IsApiValidRequest = Config.AppSetting<bool>("IsApiValidRequest", false);
            SmtpServer = Config.AppSetting("SmtpServer");
            SmtpPort = Config.AppSetting<int>("SmtpPort");
            SmtpFromName = Config.AppSetting("SmtpFromName");
            SmtpFromEmail = Config.AppSetting("SmtpFromEmail");
            SmtpFromPassword = Config.AppSetting("SmtpFromPassword");
            ExceptionMails = Config.AppSetting("ExceptionMails");
        }

        /// <summary>
        /// 取得应用程序全局设定类单例对象。
        /// </summary>
        public static Setting Instance { get; } = new Setting();

        /// <summary>
        /// 取得当前应用程序信息。
        /// </summary>
        public AppInfo App { get; }

        /// <summary>
        /// 取得平台 Api 请求地址。
        /// </summary>
        public string ApiPlatformUrl { get; }

        /// <summary>
        /// 取得当前应用程序 Api 请求地址。
        /// </summary>
        public string ApiBaseUrl { get; }

        /// <summary>
        /// 取得当前应用程序 Api 请求时是否验证请求参数签名信息。
        /// </summary>
        public bool IsApiValidRequest { get; }

        /// <summary>
        /// 取得当前应用程序是否是单机版。
        /// </summary>
        public bool IsMonomer { get; }

        /// <summary>
        /// 取得当前应用程序 SMTP 邮件服务器。
        /// </summary>
        public string SmtpServer { get; }

        /// <summary>
        /// 取得当前应用程序 SMTP 邮件服务器端口。
        /// </summary>
        public int SmtpPort { get; }

        /// <summary>
        /// 取得当前应用程序邮件发送者名称。
        /// </summary>
        public string SmtpFromName { get; }

        /// <summary>
        /// 取得当前应用程序邮件发送者邮箱。
        /// </summary>
        public string SmtpFromEmail { get; }

        /// <summary>
        /// 取得当前应用程序邮件发送者邮箱密码。
        /// </summary>
        public string SmtpFromPassword { get; }

        /// <summary>
        /// 取得当前应用程序异常邮件接收者邮箱。
        /// </summary>
        public string ExceptionMails { get; set; }
    }
}
