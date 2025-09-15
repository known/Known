namespace Known.Extensions;

static class PlatformExtension
{
    internal static Task<List<LanguageInfo>> GetLanguagesAsync(this Database db)
    {
        return db.Query<SysLanguage>().ToListAsync<LanguageInfo>();
    }

    internal static async Task<List<ButtonInfo>> GetButtonsAsync(this Database db)
    {
        var datas = await db.GetConfigAsync<List<ButtonInfo>>(Constant.KeyButton, true);
        datas ??= [];
        foreach (var item in Config.Actions)
        {
            if (datas.Exists(d => d.Id == item.Id))
                continue;

            datas.Add(item.ToButton());
        }
        return datas;
    }

    internal static async Task SaveUserAsync(this Database db, InstallInfo info)
    {
        var userName = info.AdminName.ToLower();
        var user = await db.QueryAsync<SysUser>(d => d.UserName == userName);
        user ??= new SysUser();
        user.AppId = Config.App.Id;
        user.CompNo = info.CompNo;
        user.OrgNo = info.CompNo;
        user.UserName = userName;
        user.Password = Utils.ToMd5(info.AdminPassword);
        user.Name = info.AdminName;
        user.EnglishName = info.AdminName;
        user.Gender = "Male";
        user.Role = "Admin";
        user.Enabled = true;
        Config.OnNewUser?.Invoke(db, user);
        await db.SaveAsync(user);
    }
}