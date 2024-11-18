namespace Known.Components;

/// <summary>
/// CSS样式表标签组件类。
/// </summary>
public class KStyleSheet : ComponentBase
{
    internal static readonly List<string> Items = [];

    /// <summary>
    /// 添加CSS样式表文件到集合中。
    /// </summary>
    /// <param name="fileName">CSS样式表文件。</param>
    public static void AddStyle(string fileName)
    {
        if (Items.Contains(fileName))
            return;

        Items.Add(fileName);
    }

    /// <summary>
    /// 呈现脚本HTML标签。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        foreach (var item in Items)
        {
            var href = Config.GetStaticFileUrl(item);
            builder.Markup($"<link rel=\"stylesheet\" href=\"{href}\" />");
        }
    }
}