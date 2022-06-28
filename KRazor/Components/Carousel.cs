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

public class Carousel : BaseComponent
{
    private System.Timers.Timer timer;
    private int curIndex = 0;

    [Parameter] public int Interval { get; set; } = 3000;
    [Parameter] public bool ShowSnk { get; set; } = true;
    [Parameter] public string[] Images { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        timer = new(Interval);
        timer.Elapsed += (sender, eventArgs) => OnTimerCallback();
        timer.Start();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("slider", attr =>
        {
            BuildItems(builder);
            BuildSnks(builder);
        });
    }

    private void BuildItems(RenderTreeBuilder builder)
    {
        if (Images == null || Images.Length == 0)
            return;

        for (int i = 0; i < Images.Length; i++)
        {
            var item = Images[i];
            builder.Div($"slider-item {ActiveItem(i)}", attr =>
            {
                builder.Img(attr => attr.Src(item));
            });
        }
    }

    private void BuildSnks(RenderTreeBuilder builder)
    {
        if (!ShowSnk)
            return;

        if (Images == null || Images.Length == 0)
            return;

        builder.Div("slider-snk", attr =>
        {
            for (int i = 0; i < Images.Length; i++)
            {
                builder.Span(attr => attr.Class(ActiveSnk(i)));
            }
        });
    }

    private void OnTimerCallback()
    {
        _ = InvokeAsync(() =>
        {
            curIndex++;
            if (curIndex >= Images.Length)
                curIndex = 0;
            StateHasChanged();
        });
    }

    private string ActiveItem(int index) => index == curIndex ? "active fadeIn animated" : "";
    private string ActiveSnk(int index) => index == curIndex ? "active" : "";
}
