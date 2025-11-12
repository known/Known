namespace Known.Services;

/// <summary>
/// 管理后台数据服务接口。
/// </summary>
public partial interface IAdminService : IService
{
    /// <summary>
    /// 异步设置呈现模式。
    /// </summary>
    /// <param name="mode">呈现模式。</param>
    /// <returns></returns>
    Task<Result> SetRenderModeAsync(string mode);

    /// <summary>
    /// 异步获取系统初始数据信息，语言等。
    /// </summary>
    /// <returns>系统初始数据信息。</returns>
    [Anonymous] Task<InitialInfo> GetInitialAsync();

    /// <summary>
    /// 异步获取系统附件列表。
    /// </summary>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <returns>系统附件列表。</returns>
    Task<List<AttachInfo>> GetFilesAsync(string bizId);

    /// <summary>
    /// 异步删除单条系统附件。
    /// </summary>
    /// <param name="info">系统附件对象。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteFileAsync(AttachInfo info);

    /// <summary>
    /// 异步添加系统日志。
    /// </summary>
    /// <param name="info">系统日志</param>
    /// <returns>添加结果。</returns>
    Task<Result> AddLogAsync(LogInfo info);

    /// <summary>
    /// 异步删除菜单信息。
    /// </summary>
    /// <param name="info">菜单信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> DeleteMenuAsync(MenuInfo info);

    /// <summary>
    /// 异步保存菜单信息。
    /// </summary>
    /// <param name="info">菜单信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveMenuAsync(MenuInfo info);
}

[Client]
partial class AdminClient(HttpClient http) : ClientBase(http), IAdminService
{
    public Task<Result> SetRenderModeAsync(string mode) => Http.PostAsync($"/Admin/SetRenderMode?mode={mode}");
    public Task<InitialInfo> GetInitialAsync() => Http.GetAsync<InitialInfo>("/Admin/GetInitial");
    public Task<List<AttachInfo>> GetFilesAsync(string bizId) => Http.GetAsync<List<AttachInfo>>($"/Admin/GetFiles?bizId={bizId}");
    public Task<Result> DeleteFileAsync(AttachInfo info) => Http.PostAsync("/Admin/DeleteFile", info);
    public Task<Result> AddLogAsync(LogInfo info) => Http.PostAsync("/Admin/AddLog", info);
    public Task<Result> DeleteMenuAsync(MenuInfo info) => Http.PostAsync("/Admin/DeleteMenu", info);
    public Task<Result> SaveMenuAsync(MenuInfo info) => Http.PostAsync("/Admin/SaveMenu", info);
}

[WebApi, Service]
partial class AdminService(Context context, INotifyService notify) : ServiceBase(context), IAdminService
{
    public Task<Result> SetRenderModeAsync(string mode)
    {
        Config.CurrentMode = Utils.ConvertTo<RenderType>(mode);
        return Result.SuccessAsync(Language.SetSuccess, Config.CurrentMode);
    }

    [Anonymous]
    public async Task<InitialInfo> GetInitialAsync()
    {
        var database = Database;
        if (Language.Settings == null || Language.Settings.Count == 0)
            await AppHelper.LoadLanguagesAsync(database);

        var sys = await database.GetSystemAsync(true);
        var info = new InitialInfo
        {
            HostUrl = Config.HostUrl,
            IsInstalled = sys != null,
            LanguageSettings = Language.Settings,
            Languages = Language.Datas
        };
        if (sys != null)
        {
            info.System = sys.Clone();
            info.System.ProductId = null;
            info.System.ProductKey = null;
            info.System.UserDefaultPwd = null;
        }
        CoreConfig.System = sys;
        CoreConfig.Load(info);
        if (CoreConfig.OnInitial != null)
            await CoreConfig.OnInitial.Invoke(database, info);
        return info;
    }

    public Task<List<AttachInfo>> GetFilesAsync(string bizId)
    {
        return Database.GetFilesAsync(bizId);
    }

    public async Task<Result> DeleteFileAsync(AttachInfo info)
    {
        if (info == null || string.IsNullOrWhiteSpace(info.Path))
            return Result.Error(Language.TipFileNotExists);

        var oldFiles = new List<string>();
        await Database.DeleteFileAsync(info, oldFiles);
        AttachFile.DeleteFiles(oldFiles);
        return Result.Success(Language.DeleteSuccess);
    }

    public Task<Result> AddLogAsync(LogInfo info)
    {
        var user = Context.CurrentUser;
        user.LastPage = info.Target;
        Cache.RefreshUser(user);
        notify.NotifyOnlineAsync();
        return Database.AddLogAsync(info);
    }

    public async Task<Result> DeleteMenuAsync(MenuInfo info)
    {
        var database = Database;
        var module = await database.QueryByIdAsync<SysModule>(info.Id);
        if (module == null)
            return Result.Error(Language.TipModuleNotExists);

        return await database.TransactionAsync(Language.Delete, async db =>
        {
            await db.DeleteAsync(module);
            var modules = await db.Query<SysModule>().Where(m => m.ParentId == info.ParentId).ToListAsync();
            if (modules != null && modules.Count > 0)
            {
                var index = 1;
                foreach (var item in modules)
                {
                    item.Sort = index++;
                    await db.SaveAsync(item);
                }
            }
        });
    }

    public async Task<Result> SaveMenuAsync(MenuInfo info)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<SysModule>(info.Id);
        model ??= new SysModule();
        model.FillModel(info);

        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        if (string.IsNullOrWhiteSpace(model.Icon))
            model.Icon = "";//AntDesign不识别null值

        if (string.IsNullOrWhiteSpace(model.Code))
            model.Code = model.Name;
        model.LayoutData = Utils.ToJson(info.Layout);
        model.PluginData = info.Plugins?.ZipDataString();
        return await Database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
            info.Id = model.Id;
        }, info);
    }
}