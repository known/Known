namespace Known.Components;

/// <summary>
/// 左侧列表和右侧表格关联组件类。
/// </summary>
/// <typeparam name="TItem">表格数据类型。</typeparam>
public class KListTable<TItem> : BaseComponent where TItem : class, new()
{
    /// <summary>
    /// 取得或设置列表项呈现模板。
    /// </summary>
    [Parameter] public RenderFragment<CodeInfo> ListTemplate { get; set; }

    /// <summary>
    /// 取得或设置列表数据源。
    /// </summary>
    [Parameter] public List<CodeInfo> ListData { get; set; }

    /// <summary>
    /// 取得或
    /// </summary>
    [Parameter] public EventCallback<CodeInfo> OnListClick { get; set; }

    /// <summary>
    /// 取得或设置添加数据按钮单击事件。
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> OnAddClick { get; set; }

    /// <summary>
    /// 取得或设置表格配置模型。
    /// </summary>
    [Parameter] public TableModel<TItem> Table { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-row-28", () =>
        {
            builder.Div("kui-card", () => BuildListBox(builder));
            builder.TablePage(Table);
        });
    }

    private void BuildListBox(RenderTreeBuilder builder)
    {
        builder.Component<KListBox>()
               .Set(c => c.ShowSearch, true)
               .Set(c => c.DataSource, ListData)
               .Set(c => c.ItemTemplate, ItemTemplate)
               .Set(c => c.OnItemClick, OnListClick)
               .Set(c => c.OnAddClick, OnAddClick)
               .Build();
    }

    private RenderFragment ItemTemplate(CodeInfo info)
    {
        if (ListTemplate != null)
            return b => b.Fragment(ListTemplate, info);

        return b => b.Text($"{info.Name}({info.Code})");
    }
}