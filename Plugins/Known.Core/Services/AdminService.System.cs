namespace Known.Services;

partial class AdminService
{
    public async Task<SystemInfo> GetSystemAsync()
    {
        var database = Database;
        database.EnableLog = false;
        var info = await database.GetSystemAsync();
        if (info != null)
        {
            info.ProductId = CoreOption.Instance.ProductId;
            info.ProductKey = null;
            info.UserDefaultPwd = null;
        }
        return info;
    }

    public async Task<SystemInfo> GetProductAsync()
    {
        var info = await Database.GetSystemAsync();
        if (info != null)
        {
            info.ProductId = CoreOption.Instance.ProductId;
            info.UserDefaultPwd = null;
        }
        return info;
    }

    public async Task<Result> SaveProductKeyAsync(ActiveInfo info)
    {
        var db = Database;
        if (info.Type == ActiveType.System)
        {
            var sys = await db.GetSystemAsync();
            sys.ProductId = info.ProductId;
            sys.ProductKey = info.ProductKey;
            await db.SaveSystemAsync(sys);
            CoreConfig.System = sys;
            return CoreOption.Instance.CheckSystemInfo(sys);
        }
        else if (info.Type == ActiveType.Version)
        {
            if (CoreConfig.OnActiveSystem != null)
            {
                return await CoreConfig.OnActiveSystem.Invoke(db, info);
            }
        }

        foreach (var item in CoreConfig.Actives)
        {
            var result = item.Invoke(info);
            if (!result.IsValid)
                return result;
        }
        return Result.Success("");
    }
}