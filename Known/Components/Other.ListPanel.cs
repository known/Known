namespace Known.Components;

/// <summary>
/// 左侧列表和右侧自定义关联组件类。
/// </summary>
public class KListPanel : BaseComponent
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
    /// 取得或设置右侧子组件模板。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// 设置列表数据源。
    /// </summary>
    /// <param name="data">列表数据源。</param>
    public void SetListData(List<CodeInfo> data)
    {
        ListData = data;
        StateChanged();
    }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-row-28", () =>
        {
            builder.Div("kui-card", () => BuildListBox(builder));
            builder.Fragment(ChildContent);
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