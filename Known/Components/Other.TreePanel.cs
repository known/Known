namespace Known.Components;

/// <summary>
/// 左侧树和右侧自定义关联组件类。
/// </summary>
public class KTreePanel : KPanelBase<MenuInfo>
{
    private string searchKey;

    /// <summary>
    /// 取得或设置卡片工具条。
    /// </summary>
    [Parameter] public RenderFragment Toolbar { get; set; }

    /// <summary>
    /// 取得或设置右侧子组件模板。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-row-28", () =>
        {
            BuildTreeBox(builder);
            builder.Component<CardPage>()
                   .Set(c => c.Name, Name)
                   .Set(c => c.Toolbar, Toolbar)
                   .Set(c => c.ChildContent, ChildContent)
                   .Build();
        });
    }

    private void BuildTreeBox(RenderTreeBuilder builder)
    {
        builder.Component<KTreeBox>()
               .Set(c => c.Class, "kui-card")
               .Set(c => c.ShowSearch, ShowSearch)
               .Set(c => c.AddButtonText, AddButtonText)
               .Set(c => c.OnAddClick, OnAddClick)
               .Set(c => c.DataSource, DataSource)
               .Set(c => c.ItemTemplate, ItemTemplate)
               .Set(c => c.OnItemClick, OnItemClick)
               .Build();
    }
}