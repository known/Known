using System;
using System.Linq;
using Known.Entities;

namespace Known.Core
{
    partial class PlatformService
    {
        private readonly string KeySystem = "SystemInfo";

        public InstallInfo GetInstall()
        {
            var mac = Utils.GetMacAddress();
            var id = mac.Split(':').Select(m => Convert.ToInt32(m, 16)).Sum();
            return new InstallInfo
            {
                CompNo = App.CompNo,
                CompName = App.CompName,
                AppName = App.AppName,
                ProductId = $"PM-{App.AppId}-{id:000000}",
                UserName = Constants.SysUserName
            };
        }

        public SystemInfo GetSystem()
        {
            return GetConfig<SystemInfo>(KeySystem);
        }

        public Result SaveSystem(SystemInfo info)
        {
            SaveConfig(KeySystem, info);
            return Result.Success(Language.SaveSuccess);
        }

        public Result SaveSystem(InstallInfo info)
        {
            if (info == null)
                return Result.Error("安装信息不能为空！");

            if (info.Password != info.Password1)
                return Result.Error("确认密码不一致！");

            var sys = new SystemInfo
            {
                CompNo = info.CompNo,
                CompName = info.CompName,
                AppName = info.AppName,
                ProductId = info.ProductId,
                ProductKey = info.ProductKey
            };
            SaveSystem(sys);

            var db = Database;
            db.User = new UserInfo { UserName = info.UserName };
            SaveUser(db, info);
            SaveOrganization(db, info);
            return Result.Success("初始化成功，欢迎使用！");
        }

        private static void SaveUser(Database db, InstallInfo info)
        {
            db.Save(new SysUser
            {
                AppId = App.AppId,
                CompNo = info.CompNo,
                OrgNo = info.CompNo,
                UserName = info.UserName,
                Password = Utils.ToMd5(info.Password),
                Name = "管理员",
                EnglishName = info.UserName,
                Gender = "男",
                Role = "管理员",
                Enabled = 1
            });
        }

        private static void SaveOrganization(Database db, InstallInfo info)
        {
            db.Save(new SysOrganization
            {
                AppId = App.AppId,
                CompNo = info.CompNo,
                Code = info.CompNo,
                Name = info.CompName
            });
        }
    }
}