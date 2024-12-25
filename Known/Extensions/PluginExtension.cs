namespace Known.Extensions;

static class PluginExtension
{
    internal static List<ActionInfo> ToActions(this List<PluginInfo> plugins)
    {
        var infos = new List<ActionInfo>();
        var categories = plugins.Where(p => !string.IsNullOrWhiteSpace(p.Attribute.Category))
                                .Select(p => p.Attribute.Category).Distinct().ToList();
        if (categories.Count > 0)
        {
            foreach (var category in categories)
            {
                var items = plugins.Where(p => p.Attribute.Category == category);
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
        var others = plugins.Where(p => string.IsNullOrWhiteSpace(p.Attribute.Category));
        foreach (var item in others)
        {
            infos.Add(GetAction(item));
        }
        return infos;
    }

    private static ActionInfo GetAction(PluginInfo item)
    {
        return new ActionInfo
        {
            Id = item.Id,
            Name = item.Attribute.Name,
            Icon = item.Attribute.Icon ?? "file",
            Url = item.Url
        };
    }
}