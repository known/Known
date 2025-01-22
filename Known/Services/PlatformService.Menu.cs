namespace Known.Services;

public partial interface IPlatformService
{
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

partial class PlatformService
{
    public Task<Result> DeleteMenuAsync(MenuInfo info)
    {
        var module = AppData.GetModule(info.Id);
        if (module == null)
            return Result.ErrorAsync("模块不存在！");

        AppData.Data.Modules?.Remove(module);
        var modules = AppData.Data.Modules.Where(m => m.ParentId == info.ParentId).OrderBy(m => m.Sort).ToList();
        modules?.Resort();
        AppData.SaveData();
        return Result.SuccessAsync(Language.DeleteSuccess);
    }

    public Task<Result> SaveMenuAsync(MenuInfo info)
    {
        var module = AppData.GetModule(info.Id);
        if (module == null)
        {
            module = new ModuleInfo();
            if (AppData.Data.Modules == null)
                AppData.Data.Modules = [];
            AppData.Data.Modules.Add(module);
        }
        module.Id = info.Id;
        module.ParentId = info.ParentId;
        module.Name = info.Name;
        module.Icon = info.Icon;
        module.Type = info.Type;
        module.Target = info.Target;
        module.Url = info.Url;
        module.Sort = info.Sort;
        module.Layout = info.Layout;
        module.Plugins = info.Plugins;
        AppData.SaveData();
        return Result.SuccessAsync(Language.SaveSuccess, info);
    }
}

partial class PlatformClient
{
    public Task<Result> DeleteMenuAsync(MenuInfo info)
    {
        return Http.PostAsync("/Platform/DeleteMenu", info);
    }

    public Task<Result> SaveMenuAsync(MenuInfo info)
    {
        return Http.PostAsync("/Platform/SaveMenu", info);
    }
}