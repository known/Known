namespace Known.Core
{
    partial class SystemService
    {
        public SystemInfo GetSetting()
        {
            return Platform.GetSystem();
        }

        public Result SaveSetting(string data)
        {
            var info = Utils.FromJson<SystemInfo>(data);
            var result = Platform.SaveSystem(info);
            if (result.IsValid)
            {
                var user = CurrentUser;
                user.AppName = info.AppName;
                user.CompName = info.CompName;
                UserHelper.SetUser(user);
            }
            return result;
        }
    }
}