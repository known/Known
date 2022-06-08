using System.Linq;

namespace Known.Core
{
    class HomeService : ServiceBase, IService
    {
        #region Login
        [Anonymous, Route("signin")]
        public Result SignIn(string userName, string password, string captcha, string cid, bool force)
        {
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