namespace Known.Plugins;

/// <summary>
/// 插件扩展类。
/// </summary>
public static class PluginExtension
{
    /// <summary>
    /// 构建插件组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="parent">插件上级组件。</param>
    /// <param name="info">插件信息。</param>
    public static void BuildPlugin(this RenderTreeBuilder builder, BaseComponent parent, PluginInfo info)
    {
        var instance = Activator.CreateInstance(info.Type) as IPlugin;
        if (instance != null)
        {
            instance.Parent = parent;
            instance?.Render(builder, info);
        }
    }

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