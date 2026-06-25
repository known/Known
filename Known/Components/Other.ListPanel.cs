namespace Known.Components;

/// <summary>
/// 左侧列表和右侧自定义关联组件类。
/// </summary>
public class KListPanel : KPanelBase<CodeInfo>
{
    private KListBox listBox;

    /// <summary>
    /// 取得或设置右侧子组件模板。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// 设置列表数据源。
    /// </summary>
    /// <param name="data">列表数据源。</param>
    /// <param name="current">当前选中项目。</param>
    public void SetDataSource(List<CodeInfo> data, string current)
    {
        listBox?.SetDataSource(data, current);
    }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-row-28", () =>
        {
            BuildListBox(builder);
            builder.Div("kui-pane", () => builder.Fragment(ChildContent));
        });
    }

    private void BuildListBox(RenderTreeBuilder builder)
    {
        builder.Component<KListBox>()
               .Set(c => c.Class, "kui-card")
               .Set(c => c.ShowSearch, ShowSearch)
               .Set(c => c.ShowAddButton, ShowAddButton)
               .Set(c => c.AddButtonText, AddButtonText)
               .Set(c => c.OnAddClick, OnAddClick)
               .Set(c => c.DataSource, DataSource)
               .Set(c => c.ItemTemplate, ListItemTemplate)
               .Set(c => c.OnItemClick, OnItemClick)
               .Build(value => listBox = value);
    }

    private RenderFragment ListItemTemplate(CodeInfo info)
    {
        if (ItemTemplate != null)
            return b => b.Fragment(ItemTemplate, info);

        if (string.IsNullOrWhiteSpace(info.Name))
            return b => b.Text(info.Code);

        return b => b.Text($"{info.Name}({info.Code})");
    }
}