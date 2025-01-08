﻿namespace Known.Services;

/// <summary>
/// 管理后台数据服务接口。
/// </summary>
public interface IAdminService : IService
{
    #region Config
    /// <summary>
    /// 异步判断系统是否需要安装。
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous] Task<bool> GetInstallAsync();

    /// <summary>
    /// 异步获取系统配置数据。
    /// </summary>
    /// <param name="key">配置数据键。</param>
    /// <returns>配置数据JSON字符串。</returns>
    Task<string> GetConfigAsync(string key);

    /// <summary>
    /// 异步保存系统配置数据。
    /// </summary>
    /// <param name="info">系统配置数据信息。</param>
    /// <returns></returns>
    Task<Result> SaveConfigAsync(ConfigInfo info);
    #endregion

    #region Auth
    /// <summary>
    /// 异步用户登录。
    /// </summary>
    /// <param name="info">登录表单对象。</param>
    /// <returns>登录结果。</returns>
    [AllowAnonymous] Task<Result> SignInAsync(LoginFormInfo info);

    /// <summary>
    /// 异步注销登录。
    /// </summary>
    /// <returns>注销结果。</returns>
    Task<Result> SignOutAsync();

    /// <summary>
    /// 异步获取系统后台首页数据。
    /// </summary>
    /// <returns>后台首页数据。</returns>
    Task<AdminInfo> GetAdminAsync();
    #endregion

    #region User
    /// <summary>
    /// 异步获取用户信息。
    /// </summary>
    /// <param name="userName">用户名。</param>
    /// <returns>用户信息。</returns>
    Task<UserInfo> GetUserAsync(string userName);

    /// <summary>
    /// 异步根据ID获取用户信息。
    /// </summary>
    /// <param name="userId">用户ID。</param>
    /// <returns>用户信息。</returns>
    Task<UserInfo> GetUserByIdAsync(string userId);
    #endregion

    #region Setting
    /// <summary>
    /// 异步获取用户设置信息JSON。
    /// </summary>
    /// <param name="bizType">设置业务类型。</param>
    /// <returns>用户设置信息JSON。</returns>
    Task<string> GetUserSettingAsync(string bizType);

    /// <summary>
    /// 异步保存用户业务设置信息，如：高级查询。
    /// </summary>
    /// <param name="info">设置表单信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveUserSettingFormAsync(SettingFormInfo info);
    #endregion

    #region File
    /// <summary>
    /// 异步获取系统附件列表。
    /// </summary>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <returns>系统附件列表。</returns>
    Task<List<AttachInfo>> GetFilesAsync(string bizId);

    /// <summary>
    /// 异步删除单条系统附件。
    /// </summary>
    /// <param name="file">系统附件对象。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteFileAsync(AttachInfo file);
    #endregion

    #region Log
    /// <summary>
    /// 异步添加系统日志。
    /// </summary>
    /// <param name="log">系统日志</param>
    /// <returns>添加结果。</returns>
    Task<Result> AddLogAsync(LogInfo log);
    #endregion
}

class AdminService(Context context) : ServiceBase(context), IAdminService
{
    private static readonly Dictionary<string, string> Configs = [];

    public Task<bool> GetInstallAsync()
    {
        return Task.FromResult(false);
    }

    public Task<string> GetConfigAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return Task.FromResult("");

        Configs.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }

    public Task<Result> SaveConfigAsync(ConfigInfo info)
    {
        if (!string.IsNullOrWhiteSpace(info.Key))
            Configs[info.Key] = Utils.ToJson(info.Value);
        return Result.SuccessAsync("保存成功！");
    }

    public Task<Result> SignInAsync(LoginFormInfo info)
    {
        if (string.IsNullOrWhiteSpace(info.UserName))
            return Result.ErrorAsync("用户名不能为空！");

        var user = new UserInfo
        {
            UserName = info.UserName,
            Name = "管理员",
            AvatarUrl = "img/face1.png",
            Token = Utils.GetGuid()
        };
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

class AdminClient(HttpClient http) : ClientBase(http), IAdminService
{
    public Task<bool> GetInstallAsync()
    {
        return Http.GetAsync<bool>("/Admin/GetInstall");
    }

    public Task<string> GetConfigAsync(string key)
    {
        return Http.GetStringAsync($"/Admin/GetConfig?key={key}");
    }

    public Task<Result> SaveConfigAsync(ConfigInfo info)
    {
        return Http.PostAsync("/Admin/SaveConfig", info);
    }

    public Task<Result> SignInAsync(LoginFormInfo info)
    {
        return Http.PostAsync("/Admin/SignIn", info);
    }

    public Task<Result> SignOutAsync()
    {
        return Http.PostAsync("/Admin/SignOut");
    }

    public Task<AdminInfo> GetAdminAsync()
    {
        return Http.GetAsync<AdminInfo>("/Admin/GetAdmin");
    }

    public Task<UserInfo> GetUserAsync(string userName)
    {
        return Http.GetAsync<UserInfo>($"/Admin/GetUser?userName={userName}");
    }

    public Task<UserInfo> GetUserByIdAsync(string userId)
    {
        return Http.GetAsync<UserInfo>($"/Admin/GetUserById?userId={userId}");
    }

    public Task<string> GetUserSettingAsync(string bizType)
    {
        return Http.GetStringAsync($"/Admin/GetUserSetting?bizType={bizType}");
    }

    public Task<Result> SaveUserSettingFormAsync(SettingFormInfo info)
    {
        return Http.PostAsync("/Admin/SaveConfig", info);
    }

    public Task<List<AttachInfo>> GetFilesAsync(string bizId)
    {
        return Http.GetAsync<List<AttachInfo>>($"/Admin/GetFiles?bizId={bizId}");
    }

    public Task<Result> DeleteFileAsync(AttachInfo file)
    {
        return Http.PostAsync("/Admin/DeleteFile", file);
    }

    public Task<Result> AddLogAsync(LogInfo log)
    {
        return Http.PostAsync("/Admin/AddLog", log);
    }
}