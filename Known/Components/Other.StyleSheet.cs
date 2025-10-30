namespace Known.Components;

/// <summary>
/// CSS样式表标签组件类。
/// </summary>
public class KStyleSheet : ComponentBase
{
    private static readonly Dictionary<string, string> Items = [];

    /// <summary>
    /// 添加LESS样式表文件到集合中。
    /// </summary>
    /// <param name="fileName">LESS样式表文件。</param>
    public static void AddLess(string fileName)
    {
        if (Items.ContainsKey(fileName))
            return;

        Items[fileName] = "less";
    }

    /// <summary>
    /// 添加CSS样式表文件到集合中。
    /// </summary>
    /// <param name="fileName">CSS样式表文件。</param>
    public static void AddStyle(string fileName)
    {
        if (Items.ContainsKey(fileName))
            return;

        Items[fileName] = "stylesheet";
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        foreach (var item in Items)
        {
            var href = Config.GetStaticFileUrl(item.Key);
            if (item.Value == "less")
                builder.Markup($"<link rel=\"stylesheet/less\" type=\"text/css\" href=\"{href}\" />");
            else
                builder.Markup($"<link rel=\"{item.Value}\" href=\"{href}\" />");
        }
    }
}