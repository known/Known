namespace Known.Services;

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