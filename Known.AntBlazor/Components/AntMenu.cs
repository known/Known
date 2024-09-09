namespace Known.AntBlazor.Components;

/// <summary>
/// 自定义Ant菜单组件类。
/// </summary>
public class AntMenu : BaseComponent
{
    /// <summary>
    /// 取得或设置是否为手风琴菜单。
    /// </summary>
    [Parameter] public bool Accordion { get; set; }

    /// <summary>
    /// 取得或设置菜单数据列表。
    /// </summary>
    [Parameter] public List<MenuInfo> Items { get; set; }

    /// <summary>
    /// 呈现菜单组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Component<Menu>()
               .Set(c => c.Mode, MenuMode.Inline)
               .Set(c => c.Accordion, Accordion)
               .Set(c => c.ChildContent, BuildMenu)
               .Build();
    }

    private void BuildMenu(RenderTreeBuilder builder)
    {
        if (Items == null || Items.Count == 0)
            return;

        BuildMenu(builder, Items);
    }

    private void BuildMenu(RenderTreeBuilder builder, List<MenuInfo> items)
    {
        foreach (var item in items)
        {
            BuildMenu(builder, item);
        }
    }

    private void BuildMenu(RenderTreeBuilder builder, MenuInfo item)
    {
        if (item.Children != null && item.Children.Count > 0)
        {
            builder.Component<SubMenu>()
                   .Set(c => c.Key, item.Id)
                   .Set(c => c.TitleTemplate, b => BuildTitle(b, item))
                   .Set(c => c.ChildContent, b => BuildMenu(b, item.Children))
                   .Build();
        }
        else
        {
            BuildMenuItem(builder, item);
        }
    }

    private void BuildTitle(RenderTreeBuilder builder, MenuInfo item)
    {
        builder.Span().Children(() => BuildItemName(builder, item)).Close();
    }

    private void BuildMenuItem(RenderTreeBuilder builder, MenuInfo item)
    {
        builder.Component<MenuItem>()
               .Set(c => c.Key, item.Id)
               .Set(c => c.RouterLink, item.RouteUrl)
               .Set(c => c.ChildContent, b => BuildItemName(b, item))
               .Build();
    }

    private void BuildItemName(RenderTreeBuilder builder, MenuInfo item)
    {
        var itemName = Language.GetString(item);
        builder.Icon(item.Icon);
        builder.Span(itemName);
    }
}