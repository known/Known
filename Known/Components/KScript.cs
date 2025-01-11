namespace Known.Components;

/// <summary>
/// JS脚本标签组件类。
/// </summary>
public class KScript : ComponentBase
{
    internal static readonly List<string> Items = [];

    /// <summary>
    /// 添加JS脚本文件到集合中。
    /// </summary>
    /// <param name="fileName">JS脚本文件。</param>
    public static void AddScript(string fileName)
    {
        if (Items.Contains(fileName))
            return;

        Items.Add(fileName);
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        foreach (var item in Items)
        {
            var src = Config.GetStaticFileUrl(item);
            builder.Markup($"<script src=\"{src}\"></script>");
        }
    }
}