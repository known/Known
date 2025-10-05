namespace Known.Extensions;

static class PlatformExtension
{
    internal static PagingResult<ButtonInfo> ToQueryResult(this List<ButtonInfo> infos, PagingCriteria criteria)
    {
        var query = criteria.Query.FirstOrDefault(q => q.Id == nameof(ButtonInfo.Id));
        if (query != null && !string.IsNullOrWhiteSpace(query.Value))
        {
            if (query.Type == QueryType.NotIn)
                infos = [.. infos.Where(b => !query.Value.Split(',').Contains(b.Id))];
            else
                infos = [.. infos.Where(b => b.Id.Contains(query.Value))];
        }
        //infos = [.. infos.Contains(m => m.Id, criteria)];
        infos = [.. infos.Contains(m => m.Name, criteria)];
        //infos = [.. infos.Contains(m => m.Position, criteria)];
        var position = criteria.GetQueryValue(nameof(ButtonInfo.Position));
        if (!string.IsNullOrWhiteSpace(position))
            infos = [.. infos.Where(b => b.Position?.Contains(position) == true)];
        return infos.ToPagingResult(criteria);
    }

    internal static Task<List<SysLanguage>> GetLanguagesAsync(this Database db)
    {
        return db.QueryListAsync<SysLanguage>();
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
        var actions = Config.Actions.ToList();
        var datas = await db.GetConfigAsync<List<ButtonInfo>>(Constants.KeyButton, true);
        if (datas != null && datas.Count > 0)
        {
            var items = datas.Where(d => !actions.Exists(a => a.Id == d.Id)).Select(d => d.ToAction()).ToList();
            if (items != null && items.Count > 0)
                actions.AddRange(items);
        }
        return actions;
    }

    internal static string ZipDataString(this List<PluginInfo> plugins)
    {
        if (plugins == null || plugins.Count == 0)
            return string.Empty;

        return ZipHelper.ZipDataAsString(plugins);
    }
}