namespace Sample.WasmApi.Services;

[WebApi]
class AdminService(Context context) : ServiceBase(context), IAdminService
{
    private static readonly Dictionary<string, string> Configs = [];

    [AllowAnonymous]
    public Task<bool> GetInstallAsync()
    {
        return Task.FromResult(false);
    }

    public Task<string> GetConfigAsync(string key)
    {
        Configs.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }

    public Task<Result> SaveConfigAsync(ConfigInfo info)
    {
        Configs[info.Key] = Utils.ToJson(info.Value);
        return Result.SuccessAsync("保存成功！");
    }

    [AllowAnonymous]
    public Task<Result> SignInAsync(LoginFormInfo info)
    {
        if (string.IsNullOrWhiteSpace(info.UserName))
            return Result.ErrorAsync("用户名不能为空！");

        var user = new UserInfo { UserName = info.UserName, Name = info.UserName, Token = Utils.GetGuid() };
        Cache.SetUser(user);
        return Result.SuccessAsync("登录成功！", user);
    }

    public Task<Result> SignOutAsync()
    {
        return Result.SuccessAsync("注销成功！");
    }

    public Task<AdminInfo> GetAdminAsync()
    {
        var info = new AdminInfo
        {
            AppName = Config.App.Name,
            UserMenus = MenuHelper.GetUserMenus(CurrentUser, AppData.Modules)
        };
        return Task.FromResult(info);
    }

    public Task<UserInfo> GetUserAsync(string userName)
    {
        var user = Cache.GetUser(userName);
        return Task.FromResult(user);
    }

    public Task<UserInfo> GetUserByIdAsync(string userId)
    {
        return Task.FromResult(new UserInfo { Id = userId });
    }

    public Task<string> GetUserSettingAsync(string bizType)
    {
        Configs.TryGetValue(bizType, out var value);
        return Task.FromResult(value);
    }

    public Task<Result> SaveUserSettingFormAsync(SettingFormInfo info)
    {
        Configs[info.BizType] = Utils.ToJson(info.BizData);
        return Result.SuccessAsync("保存成功！");
    }

    public Task<List<AttachInfo>> GetFilesAsync(string bizId)
    {
        return Task.FromResult(new List<AttachInfo>());
    }

    public Task<Result> DeleteFileAsync(AttachInfo file)
    {
        return Result.SuccessAsync("删除成功！");
    }

    public Task<Result> AddLogAsync(LogInfo log)
    {
        return Result.SuccessAsync("添加成功！");
    }
}