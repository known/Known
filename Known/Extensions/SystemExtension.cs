namespace Known.Extensions;

static class SystemExtension
{
    internal static async Task<SystemInfo> GetUserSystemAsync(this Database db, UserInfo info = null)
    {
        if (!Config.App.IsPlatform)
            return await db.GetSystemAsync();

        var user = info ?? db.User;
        var model = await db.QueryAsync<SysCompany>(d => d.Code == user.CompNo);
        if (model != null && model.SystemData != null)
            return model.SystemData;

        return new SystemInfo
        {
            CompNo = user.CompNo,
            CompName = user.CompName,
            AppName = Config.App.Name
        };
    }
}