using AntDesign;
using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.AntBlazor.Components;

public class AntMenu : BaseComponent
{
    [Parameter] public bool Accordion { get; set; }
    [Parameter] public List<MenuItem> Items { get; set; }
    [Parameter] public Action<MenuItem> OnClick { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
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

    private void BuildMenu(RenderTreeBuilder builder, List<MenuItem> items)
    {
        foreach (var item in items)
        {
            BuildMenu(builder, item);
        }
    }

    private void BuildMenu(RenderTreeBuilder builder, MenuItem item)
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

    private void BuildTitle(RenderTreeBuilder builder, MenuItem item)
    {
        builder.Span().Children(() => BuildItemName(builder, item)).Close();
    }

    private void BuildMenuItem(RenderTreeBuilder builder, MenuItem item)
    {
        builder.Component<AntDesign.MenuItem>()
               .Set(c => c.Key, item.Id)
               .Set(c => c.OnClick, this.Callback<MouseEventArgs>(e => OnClick?.Invoke(item)))
               .Set(c => c.ChildContent, b => BuildItemName(b, item))
               .Build();
    }

    private void BuildItemName(RenderTreeBuilder builder, MenuItem item)
    {
        UI.Icon(builder, item.Icon);
        var itemName = Language.GetString(item);
        builder.Span(itemName);
    }
}