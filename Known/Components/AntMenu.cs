using AntDesign;
using Microsoft.AspNetCore.Components.Routing;

namespace Known.Components;

/// <summary>
/// 自定义Ant菜单组件类。
/// </summary>
public class AntMenu : Menu
{
    /// <summary>
    /// 取得或设置系统上下文。
    /// </summary>
    [Parameter] public UIContext Context { get; set; }

    /// <summary>
    /// 取得或设置菜单数据列表。
    /// </summary>
    [Parameter] public List<MenuInfo> Items { get; set; }

    /// <summary>
    /// 初始化菜单组件。
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        ChildContent = BuildMenu;
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
        builder.Span().Child(() => BuildItemName(builder, item)).Close();
    }

    private void BuildMenuItem(RenderTreeBuilder builder, MenuInfo item)
    {
        if (!string.IsNullOrWhiteSpace(item.Url) && item.Url.StartsWith("http"))
        {
            builder.Component<MenuItem>()
               .Set(c => c.Key, item.Id)
               .Set(c => c.OnClick, this.Callback<MouseEventArgs>(e => OnMenuClick(item)))
               .Set(c => c.ChildContent, b => BuildItemName(b, item))
               .Build();
        }
        else
        {
            builder.Component<MenuItem>()
               .Set(c => c.Key, item.Id)
               .Set(c => c.RouterMatch, NavLinkMatch.Prefix)
               .Set(c => c.RouterLink, item.RouteUrl)
               .Set(c => c.ChildContent, b => BuildItemName(b, item))
               .Build();
        }
    }

    private void BuildItemName(RenderTreeBuilder builder, MenuInfo item)
    {
        if (!string.IsNullOrWhiteSpace(item.Icon))
        {
            builder.Component<KIcon>()
                   .Set(c => c.Icon, item.Icon)
                   .Build();
        }
        var itemName = Context?.Language?.GetString(item);
        builder.Span(itemName);
    }

    private Task OnMenuClick(MenuInfo item)
    {
        return Task.CompletedTask;
    }
}