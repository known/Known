/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-04-01     KnownChen
 * ------------------------------------------------------------------------------- */

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

public class AppLayout : AppComponent
{
    private int conTop;
    private int conBottom;
    private MenuItem curItem;

    [Parameter] public string Style { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public bool ShowBack { get; set; }
    [Parameter] public bool ShowTopbar { get; set; }
    [Parameter] public bool ShowTabbar { get; set; }
    [Parameter] public MenuItem TopTool { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected virtual MenuItem[] Menus { get; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        AppContext = new AppContext { IsMobile = true };
        conTop = ShowTopbar ? 50 : 0;
        conBottom = ShowTabbar ? 50 : 0;
        if (ShowTabbar)
        {
            var id = Navigation.Uri.Replace(Navigation.BaseUri, "");
            curItem = Menus.FirstOrDefault(m => m.Id == id);
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<CascadingValue<AppContext>>(attr =>
        {
				attr.Add(nameof(CascadingValue<AppContext>.IsFixed), true)
                    .Add(nameof(CascadingValue<AppContext>.Value), AppContext)
				    .Add(nameof(CascadingValue<AppContext>.ChildContent), delegate (RenderTreeBuilder builder1)
                    {
                        BuildTopbar(builder1);
                        BuildContent(builder1);
                        BuildTabbar(builder1);
                    });
        });

        builder.Component<DialogContainer>();
    }

    private void BuildTopbar(RenderTreeBuilder builder)
    {
        if (!ShowTopbar)
            return;

        builder.Component<Topbar>(attr =>
        {
            attr.Add(nameof(Topbar.ShowBack), ShowBack)
                .Add(nameof(Topbar.Title), Title)
                .Add(nameof(Topbar.Tool), TopTool);
        });
    }

    private void BuildContent(RenderTreeBuilder builder)
    {
        var style = new StyleBuilder()
            .Add("top", $"{conTop}px")
            .Add("bottom", $"{conBottom}px")
            .Build();
        builder.Div($"content {Style}", attr =>
        {
            attr.Style(style);
            builder.Fragment(ChildContent);
        });
    }

    private void BuildTabbar(RenderTreeBuilder builder)
    {
        if (!ShowTabbar)
            return;

        builder.Component<Tabbar>(attr =>
        {
            attr.Add(nameof(Tabbar.Items), Menus)
                .Add(nameof(Tabbar.CurItem), curItem)
                .Add(nameof(Tabbar.OnChanged), Callback<MenuItem>(OnMenuChanged));
        });
    }

    private void OnMenuChanged(MenuItem menu)
    {
        Navigation.NavigateTo(menu.Id);
    }
}
