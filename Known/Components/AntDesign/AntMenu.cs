using AntDesign;

namespace Known.Components;

/// <summary>
/// 自定义Ant菜单组件类。
/// </summary>
public class AntMenu : Menu
{
    private bool isLoading = true;

    /// <summary>
    /// 取得或设置系统上下文。
    /// </summary>
    [Parameter] public UIContext Context { get; set; }

    /// <summary>
    /// 取得或设置菜单数据列表。
    /// </summary>
    [Parameter] public List<MenuInfo> Items { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        DefaultOpenKeys = [Items?.FirstOrDefault()?.Id];
        ChildContent = BuildMenu;
    }

    /// <summary>
    /// 设置菜单数据源。
    /// </summary>
    /// <param name="items">菜单信息列表。</param>
    /// <returns></returns>
    public void SetItems(List<MenuInfo> items)
    {
        isLoading = false;
        Items = items;
        DefaultOpenKeys = [Items?.FirstOrDefault()?.Id];
        StateHasChanged();
    }

    private void BuildMenu(RenderTreeBuilder builder)
    {
        if (Items != null && Items.Count > 0)
        {
            BuildMenu(builder, Items);
            return;
        }
        else if (isLoading)
        {
            builder.Li("kui-p10", "Loading...");
        }
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
                   .Set(c => c.Disabled, !item.Enabled)
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
        builder.Span().Child(() => BuildItemName(builder, item));
    }

    private void BuildMenuItem(RenderTreeBuilder builder, MenuInfo item)
    {
        if (item.Url?.StartsWith("http") == true && item.Target != nameof(LinkTarget.IFrame))
        {
            builder.Component<MenuItem>()
               .Set(c => c.Key, item.Id)
               .Set(c => c.Disabled, !item.Enabled)
               .Set(c => c.ChildContent, b => BuildItemLink(b, item))
               .Build();
        }
        else
        {
            builder.Component<MenuItem>()
               .Set(c => c.Key, item.Id)
               .Set(c => c.Disabled, !item.Enabled)
               .Set(c => c.OnClick, this.Callback<MouseEventArgs>(e => OnMenuItemClick(item)))
               //.Set(c => c.RouterMatch, NavLinkMatch.Prefix)
               //.Set(c => c.RouterLink, item.RouteUrl)
               .Set(c => c.ChildContent, b => BuildItemName(b, item))
               .Build();
        }
    }

    private void BuildItemLink(RenderTreeBuilder builder, MenuInfo item)
    {
        builder.Link().Href(item.Url).Set("target", "_blank")
               .Child(() => BuildItemName(builder, item));
    }

    private void BuildItemName(RenderTreeBuilder builder, MenuInfo item)
    {
        var itemName = Context?.Language?.GetString(item);
        builder.IconName(item.Icon, itemName);
    }

    private void OnMenuItemClick(MenuInfo item)
    {
        Context.NavigateTo(item);
    }
}