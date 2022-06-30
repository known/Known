/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-06-29     KnownChen    初始化
 * ------------------------------------------------------------------------------- */

using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;

namespace Known.Razor;

public class AppRouter : BaseComponent
{
    [Parameter] public bool IsMobile { get; set; }
    [Parameter] public Assembly Assembly { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        AppContext = new AppContext
        {
            IsMobile = IsMobile
        };
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<CascadingValue<AppContext>>(attr =>
        {
            attr.Add(nameof(CascadingValue<AppContext>.Value), AppContext)
                .Add(nameof(CascadingValue<AppContext>.ChildContent), BuildTree(b => BuildRouter(b))); ;
        });
        builder.Component<DialogContainer>();
    }

    private void BuildRouter(RenderTreeBuilder builder)
    {
        builder.Component<Router>(attr =>
        {
            attr.Add(nameof(Router.AppAssembly), Assembly)
                .Add(nameof(Router.Found), BuildTree<RouteData>((b, r) => BuildFound(b, r)))
                .Add(nameof(Router.NotFound), BuildTree(b => BuildNotFound(b)));
        });
    }

    private static void BuildFound(RenderTreeBuilder builder, RouteData data)
    {
        builder.Component<RouteView>(attr =>
        {
            attr.Add(nameof(RouteView.RouteData), data);
        });
    }

    private static void BuildNotFound(RenderTreeBuilder builder)
    {
        builder.Component<Error>(attr =>
        {
            attr.Add(nameof(Error.Type), "404");
        });
    }
}