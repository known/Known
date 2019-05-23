namespace Known
{
    public class Setting
    {
        private static readonly Setting instance = new Setting();

        private Setting()
        {
            AppId = Config.AppSetting("AppId");
            AppName = Config.AppSetting("AppName");
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

        public static Setting Instance
        {
            get { return instance; }
        }

        public string AppId { get; }
        public string AppName { get; }
        public string ApiPlatformUrl { get; }
        public string ApiBaseUrl { get; }
        public bool IsApiValidRequest { get; }
        public bool IsMonomer { get; }
        public string SmtpServer { get; }
        public int SmtpPort { get; }
        public string SmtpFromName { get; }
        public string SmtpFromEmail { get; }
        public string SmtpFromPassword { get; }
        public string ExceptionMails { get; set; }
    }
}
