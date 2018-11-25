namespace Known
{
    public class Setting
    {
        private static readonly Setting instance = new Setting();

        private Setting()
        {
            SystemId = Config.AppSetting("SystemId");
            SystemName = Config.AppSetting("SystemName");
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

        public bool IsDbAudit { get; }
        public string SystemId { get; }
        public string SystemName { get; }
        public string SmtpServer { get; }
        public int SmtpPort { get; }
        public string SmtpFromName { get; }
        public string SmtpFromEmail { get; }
        public string SmtpFromPassword { get; }
        public string ExceptionMails { get; set; }
    }
}
