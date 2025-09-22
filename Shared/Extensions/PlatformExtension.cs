namespace Known.Extensions;

static class PlatformExtension
{
    internal static Task<List<LanguageInfo>> GetLanguagesAsync(this Database db)
    {
        return db.Query<SysLanguage>().ToListAsync<LanguageInfo>();
    }

    internal static async Task<List<ButtonInfo>> GetButtonsAsync(this Database db)
    {
        var datas = await db.GetConfigAsync<List<ButtonInfo>>(Constants.KeyButton, true);
        datas ??= [];
        foreach (var item in Config.Actions)
        {
            if (datas.Exists(d => d.Id == item.Id))
                continue;

            datas.Add(item.ToButton());
        }
        return datas;
    }

    internal static async Task<List<ActionInfo>> GetActionsAsync(this Database db)
    {
        var actions = Config.Actions;
        var datas = await db.GetConfigAsync<List<ButtonInfo>>(Constants.KeyButton, true);
        if (datas != null && datas.Count > 0)
        {
            var items = datas.Where(d => !actions.Exists(a => a.Id == d.Id)).Select(d => d.ToAction()).ToList();
            if (items != null && items.Count > 0)
                actions.AddRange(items);
        }
        return actions;
    }
}