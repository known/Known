namespace Known.Components;

/// <summary>
/// JS脚本标签组件类。
/// </summary>
public class KScript : ComponentBase
{
    internal static readonly Dictionary<string, string> Items = [];

    /// <summary>
    /// 添加JS模块脚本文件到集合中。
    /// </summary>
    /// <param name="fileName">JS模块脚本文件。</param>
    public static void AddModule(string fileName)
    {
        if (Items.ContainsKey(fileName))
            return;

        Items[fileName] = "module";
    }

    /// <summary>
    /// 添加JS脚本文件到集合中。
    /// </summary>
    /// <param name="fileName">JS脚本文件。</param>
    /// <param name="type">类型。</param>
    public static void AddScript(string fileName, string type = "")
    {
        if (Items.ContainsKey(fileName))
            return;

        Items[fileName] = type;
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        foreach (var item in Items)
        {
            var src = Config.GetStaticFileUrl(item.Key);
            if (!string.IsNullOrWhiteSpace(item.Value))
                builder.Markup($"<script type=\"{item.Value}\" src=\"{src}\"></script>");
            else
                builder.Markup($"<script src=\"{src}\"></script>");
        }
    }
}