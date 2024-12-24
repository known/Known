namespace Known.Extensions;

static class PluginExtension
{
    internal static List<ActionInfo> ToActions(this List<PluginAttribute> plugins)
    {
        var infos = new List<ActionInfo>();
        var categories = plugins.Where(p => !string.IsNullOrWhiteSpace(p.Category))
                                .Select(p => p.Category).Distinct().ToList();
        if (categories.Count > 0)
        {
            foreach (var category in categories)
            {
                var items = plugins.Where(p => p.Category == category);
                var info = new ActionInfo
                {
                    Id = category,
                    Icon = "folder",
                    Name = category
                };
                info.Children.AddRange(items.Select(GetAction));
                infos.Add(info);
            }
        }
        var others = plugins.Where(p => string.IsNullOrWhiteSpace(p.Category));
        foreach (var item in others)
        {
            infos.Add(GetAction(item));
        }
        return infos;
    }

    private static ActionInfo GetAction(PluginAttribute item)
    {
        return new ActionInfo
        {
            Id = item.Id,
            Name = item.Name,
            Icon = item.Icon ?? "file",
            Url = item.Url
        };
    }
}