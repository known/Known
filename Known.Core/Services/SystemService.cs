namespace Known.Core.Services;

class SystemService : BaseService
{
    private const string KeySystem = "SystemInfo";

    internal SystemService(Context context) : base(context) { }

    internal string GetConfig(string key) => PlatformRepository.GetConfig(Database, Config.AppId, key);

    internal Result SaveConfig(ConfigInfo info)
    {
        SaveConfig(Database, info.Key, info.Value);
        return Result.Success("保存成功！");
    }

    internal static SystemInfo GetSystem(Database db) => GetConfig<SystemInfo>(db, KeySystem);

    internal Result CheckInstall()
    {
        var data = new CheckInfo();
        var info = GetSystem(Database);
        if (info == null)
        {
            data.IsCheckKey = false;
            data.IsInstalled = false;
            data.Install = GetInstall();
            return Result.Error("产品未安装！", data);
        }

        var result = CheckKey();
        data.IsCheckKey = result.IsValid;
        return Result.Success("产品已安装！", data);
    }
    
    internal SystemInfo GetSystem()
    {
        var info = GetSystem(Database);
        if (info != null)
        {
            var install = GetInstall();
            info.ProductId = install.ProductId;
            info.ProductKey = install.ProductKey;
        }
        return info;
    }

    internal Result UpdateKey(InstallInfo info)
    {
        if (info == null)
            return Result.Error("安装信息不能为空！");

        if (!Utils.HasNetwork())
            return Result.Error("电脑未联网，无法获取产品密钥！");

        var result = PlatformHelper.UpdateKey?.Invoke(info);
        return result ?? Result.Success("");
    }

    internal Result SaveInstall(InstallInfo info)
    {
        if (info == null)
            return Result.Error("安装信息不能为空！");

        if (info.Password != info.Password1)
            return Result.Error("确认密码不一致！");

        var database = Database;
        var user = GetUser(info);
        var orga = GetOrganization(info);
        database.User = new UserInfo { AppId = user.AppId, CompNo = user.CompNo, UserName = user.UserName, Name = user.Name };
        var sys = new SystemInfo
        {
            CompNo = info.CompNo,
            CompName = info.CompName,
            AppName = info.AppName,
            ProductId = info.ProductId,
            ProductKey = info.ProductKey,
            UserDefaultPwd = "888888"
        };

        var path = GetProductKeyPath();
        Utils.SaveFile(path, info.ProductKey);

        var result = database.Transaction("初始化", db =>
        {
            SaveConfig(db, KeySystem, sys);
            db.Save(user);
            db.Save(orga);
        });
        if (result.IsValid)
            result = CheckInstall();
        return result;
    }

    internal Result SaveSystem(SystemInfo info)
    {
        if (KCConfig.IsPlatform)
        {
            var user = CurrentUser;
            var company = CompanyRepository.GetCompany(Database, user.CompNo);
            if (company == null)
                return Result.Error("企业不存在！");

            company.SystemData = Utils.ToJson(info);
            Database.Save(company);
        }
        else
        {
            var path = GetProductKeyPath();
            Utils.SaveFile(path, info.ProductKey);
            SaveConfig(Database, KeySystem, info);
        }

        var result = CheckKey();
        return result;
    }

    private static InstallInfo GetInstall()
    {
        var app = KCConfig.App;
        var mac = Platform.GetMacAddress();
        var id = mac.Split(':').Select(m => Convert.ToInt32(m, 16)).Sum();
        var path = GetProductKeyPath();
        var info = new InstallInfo
        {
            CompNo = app.CompNo,
            CompName = app.CompName,
            AppName = Config.AppName,
            ProductId = $"PM-{Config.AppId}-{id:000000}",
            ProductKey = Utils.ReadFile(path),
            UserName = Constants.SysUserName
        };
        return info;
    }

    private static SysUser GetUser(InstallInfo info)
    {
        return new SysUser
        {
            AppId = Config.AppId,
            CompNo = info.CompNo,
            OrgNo = info.CompNo,
            UserName = info.UserName.ToLower(),
            Password = Utils.ToMd5(info.Password),
            Name = "管理员",
            EnglishName = info.UserName,
            Gender = "男",
            Role = "管理员",
            Enabled = true
        };
    }

    private static SysOrganization GetOrganization(InstallInfo info)
    {
        return new SysOrganization
        {
            AppId = Config.AppId,
            CompNo = info.CompNo,
            Code = info.CompNo,
            Name = info.CompName
        };
    }

    private static string GetProductKeyPath()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(path, "Known", $"{Config.AppId}.key");
    }

    private Result CheckKey()
    {
        var info = GetSystem();
        var result = PlatformHelper.CheckSystem?.Invoke(Database, info);
        return result ?? Result.Success("");
    }
}