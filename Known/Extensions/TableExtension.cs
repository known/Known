using AntDesign;

namespace Known.Extensions;

/// <summary>
/// 表格组件扩展类。
/// </summary>
public static class TableExtension
{
    /// <summary>
    /// 添加表格行组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="childContent">表格行子内容。</param>
    public static void AddRow(this RenderTreeBuilder builder, Action<RenderTreeBuilder> childContent)
    {
        builder.Component<TableRow>().Set(c => c.ChildContent, childContent.Invoke).Build();
    }

    /// <summary>
    /// 添加表格行组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="headers">表头表体集合。</param>
    public static void AddRow(this RenderTreeBuilder builder, params string[] headers)
    {
        builder.Component<TableRow>().Set(c => c.ChildContent, b =>
        {
            foreach (var item in headers)
            {
                b.AddHeader(item);
            }
        }).Build();
    }

    /// <summary>
    /// 添加表头组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="title">表头标题。</param>
    /// <param name="rowSpan">行跨度。</param>
    /// <param name="colSpan">列跨度。</param>
    public static void AddHeader(this RenderTreeBuilder builder, string title, int? rowSpan = null, int? colSpan = null)
    {
        builder.Component<SimpleTableHeader>()
               .Set(c => c.Title, title)
               .Set(c => c.RowSpan, rowSpan ?? 1)
               .Set(c => c.ColSpan, colSpan ?? 1)
               .Build();
    }

    internal static void SetAdminTable<TItem>(this TableModel<TItem> model) where TItem : class, new()
    {
        model.EnableEdit = false;
        model.EnableFilter = UIConfig.IsAdvAdmin;
        model.AdvSearch = UIConfig.IsAdvAdmin;
    }

    internal static void SetDevTable<TItem>(this TableModel<TItem> model) where TItem : class, new()
    {
        model.EnableEdit = false;
        model.EnableFilter = false;
        model.AdvSearch = false;
    }
}