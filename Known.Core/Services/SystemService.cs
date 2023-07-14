namespace Known.Core.Services;

class SystemService : BaseService
{
    private const string KeySystem = "SystemInfo";

    internal SystemService(Context context) : base(context) { }

    //Config
    internal string GetConfig(string key) => PlatformRepository.GetConfig(Database, Config.AppId, key);

    internal Result SaveConfig(ConfigInfo info)
    {
        SaveConfig(Database, info.Key, info.Value);
        return Result.Success("保存成功！");
    }

    //Install
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
        var company = GetCompany(info);
        var user = GetUser(info);
        var orga = GetOrganization(info);
        var sys = GetSystem(info);
        database.User = new UserInfo { AppId = user.AppId, CompNo = user.CompNo, UserName = user.UserName, Name = user.Name };
        var path = GetProductKeyPath();
        Utils.SaveFile(path, info.ProductKey);

        var result = database.Transaction("安装", db =>
        {
            SaveConfig(db, KeySystem, sys);
            db.Save(company);
            db.Save(user);
            db.Save(orga);
        });
        if (result.IsValid)
            result = CheckInstall();
        return result;
    }

    //System
    internal SystemInfo GetSystem()
    {
        var info = GetSystem(Database);
        if (info != null)
        {
            var install = GetInstall();
            info.ProductId = install.ProductId;
            info.ProductKey = install.ProductKey;

            var config = GetConfig<SystemInfo>(Database, KeySystem);
            if (config != null)
            {
                info.Copyright = config.Copyright;
                info.SoftTerms = config.SoftTerms;
            }
        }
        return info;
    }

    internal static SystemInfo GetSystem(Database db)
    {
        if (!Config.IsPlatform || db.User == null)
            return GetConfig<SystemInfo>(db, KeySystem);

        var company = CompanyRepository.GetCompany(db, db.User.CompNo);
        if (company == null)
            return GetSystem(db.User);

        return Utils.FromJson<SystemInfo>(company.SystemData);
    }

    internal Result SaveKey(SystemInfo info)
    {
        var path = GetProductKeyPath();
        Utils.SaveFile(path, info.ProductKey);
        SaveConfig(Database, KeySystem, info);
        var result = CheckKey();
        return result;
    }

    internal Result SaveSystem(SystemInfo info)
    {
        if (Config.IsPlatform)
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
            SaveConfig(Database, KeySystem, info);
        }

        return Result.Success("保存成功！");
    }

    internal Result SaveSystemConfig(SystemInfo info)
    {
        SaveConfig(Database, KeySystem, info);
        return Result.Success("保存成功！");
    }

    private static InstallInfo GetInstall()
    {
        var app = KCConfig.App;
        var path = GetProductKeyPath();
        var info = new InstallInfo
        {
            CompNo = app.CompNo,
            CompName = app.CompName,
            AppName = Config.AppName,
            ProductId = PlatformHelper.GetProductId(),
            ProductKey = Utils.ReadFile(path),
            UserName = Constants.SysUserName
        };
        return info;
    }

    private static SystemInfo GetSystem(InstallInfo info)
    {
        return new SystemInfo
        {
            CompNo = info.CompNo,
            CompName = info.CompName,
            AppName = info.AppName,
            ProductId = info.ProductId,
            ProductKey = info.ProductKey,
            UserDefaultPwd = "888888"
        };
    }

    private static SystemInfo GetSystem(UserInfo info)
    {
        return new SystemInfo
        {
            CompNo = info.CompNo,
            CompName = info.CompName,
            AppName = Config.AppName,
            UserDefaultPwd = "888888"
        };
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
            ParentId = "0",
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

    //Tenant
    internal PagingResult<SysTenant> QueryTenants(PagingCriteria criteria)
    {
        return SystemRepository.QueryTenants(Database, criteria);
    }

    internal Result SaveTenant(dynamic model)
    {
        var entity = Database.QueryById<SysTenant>((string)model.Id);
        entity ??= new SysTenant();
        entity.FillModel(model);
        var vr = entity.Validate();
        if (vr.IsValid)
        {
            entity.Code = entity.Code.ToLower();
            if (SystemRepository.ExistsTenant(Database, entity.Id, entity.Code))
                vr.AddError("账号已存在，请使用其他字符创建租户！");
        }

        if (!vr.IsValid)
            return vr;

        var info = new InstallInfo
        {
            CompNo = entity.Code,
            CompName = entity.Name,
            AppName = Config.AppName,
            UserName = entity.Code,
            Password = entity.Code
        };
        var company = GetCompany(info);
        var user = GetUser(info);
        var orga = GetOrganization(info);
        return Database.Transaction(Language.Save, db =>
        {
            if (entity.IsNew)
            {
                db.Save(company);
                db.Save(user);
                db.Save(orga);
            }
            db.Save(entity);
        }, entity);
    }

    private static SysCompany GetCompany(InstallInfo info)
    {
        var sys = GetSystem(info);
        return new SysCompany
        {
            AppId = Config.AppId,
            CompNo = info.CompNo,
            Code = info.CompNo,
            Name = info.CompName,
            SystemData = Utils.ToJson(sys)
        };
    }
}