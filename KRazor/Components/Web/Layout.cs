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

public class Layout : PageComponent
{
    private bool isHome = true;
    private readonly DialogContainer dialog = null;
    private MenuItem curMenu;
    private MenuItem curSubMenu;
    private Type componentType;

    [Parameter] public bool ShowClose { get; set; }
    [Parameter] public string AppName { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }
    [Parameter] public RenderFragment HomeTemplate { get; set; }

    protected virtual MenuItem[] Menus { get; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        AppContext = new AppContext();
        OnClickMenu();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            AppContext.Dialog = dialog;
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("app", attr =>
        {
            BuildSider(builder);
            BuildHeader(builder);
            BuildMain(builder);

            if (ShowClose)
            {
                builder.Div("close fa fa-close", attr =>
                {
                    attr.OnClick(Callback(OnClose));
                });
            }
        });

        builder.Component<DialogContainer>(attr => builder.Reference(dialog));
    }

    private void BuildSider(RenderTreeBuilder builder)
    {
        builder.Div("sider", attr =>
        {
            builder.Div("fa fa-flask logo", attr =>
            {
                attr.OnClick(Callback(e => OnClickMenu()));
            });
            builder.Component<Menu>(attr =>
            {
                attr.Add(nameof(Menu.Style), "menu menu1")
                    .Add(nameof(Menu.OnlyIcon), true)
                    .Add(nameof(Menu.CurItem), curMenu)
                    .Add(nameof(Menu.Items), Menus)
                    .Add(nameof(Menu.OnChanged), Callback<MenuItem>(e => OnClickMenu(e)));
            });
        });
    }

    private void BuildHeader(RenderTreeBuilder builder)
    {
        builder.Div("header", attr =>
        {
            builder.Div("title welcome", attr => builder.Text(AppName));
            builder.Div("time", attr => builder.Component<Timer>());
        });
    }

    private void BuildMain(RenderTreeBuilder builder)
    {
        builder.Div("main", attr =>
        {
            builder.Component<CascadingValue<AppContext>>(attr =>
            {
                attr.Add(nameof(CascadingValue<AppContext>.IsFixed), true)
                    .Add(nameof(CascadingValue<AppContext>.Value), AppContext)
                    .Add(nameof(CascadingValue<AppContext>.ChildContent), delegate (RenderTreeBuilder builder1)
                    {
                        if (isHome)
                            builder1.Fragment(HomeTemplate);
                        else
                            BuildMainContent(builder1);
                    });
            });
        });
    }

    private void BuildMainContent(RenderTreeBuilder builder)
    {
        builder.Div("title module", attr => builder.Text(curMenu.Name));
        builder.Component<Menu>(attr =>
        {
            attr.Add(nameof(Menu.Style), "menu3")
                .Add(nameof(Menu.CurItem), curSubMenu)
                .Add(nameof(Menu.Items), curMenu.Children)
                .Add(nameof(Menu.OnChanged), Callback<MenuItem>(OnClickSubMenu));
        });
        builder.Div("content", attr =>
        {
            builder.Component<DynamicComponent>(attr =>
            {
                attr.Add(nameof(DynamicComponent.Type), componentType);
            });
        });
    }

    public void OnClickMenu(MenuItem menu = null)
    {
        isHome = menu == null;
        curMenu = menu;
        if (menu != null)
        {
            if (menu.Children != null && menu.Children.Count > 0)
                OnClickSubMenu(menu.Children[0]);
            else
                componentType = menu.Type;
        }
    }

    private void OnClickSubMenu(MenuItem menu)
    {
        curSubMenu = menu;
        componentType = menu.Type;
    }
}
